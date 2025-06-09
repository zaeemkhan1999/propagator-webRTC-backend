using Microsoft.Extensions.Options;
using Propagator.Common.Services;
using Stripe;
using Stripe.Checkout;
using User = Apsy.App.Propagator.Domain.Entities.User;

namespace Apsy.App.Propagator.Application.Services;
public class PaymentService : StripeSerivceBase<Payment, StripPaymentInput, User, CustomerInput, StripFullPaymentInput>, IPaymentService
{
    public PaymentService(IPaymentRepository paymentRepository,
                  IConfiguration configuration,
                  IHttpContextAccessor httpContextAccessor,
                  ISubscriptionPlanRepository subscriptionPlanRepository,
                  IUserRepository userRepository,
                  IWebHostEnvironment env,
                  IPostRepository postRepository,
                  IAdReadRepository adRepository,
                  IArticleRepository articleRepository,
                  ISettingsRepository settingsRepository) : base(paymentRepository, httpContextAccessor)
    {
        _paymentRepository = paymentRepository;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _userRepository = userRepository;
        _env = env;
		_postRepository = postRepository;
		_adRepository = adRepository;
		_articleRepository = articleRepository;
		_settingsRepository = settingsRepository;
    }

    private readonly IPaymentRepository _paymentRepository;
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly IPostRepository _postRepository;
    private readonly IAdReadRepository _adRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly ISettingsRepository _settingsRepository;

    public ResponseBase<string> GetPublishableKey() => _configuration.GetSection("Stripe:StripeConfigPK").Value;

   
    public int GetSubscriptionPlanIdByPriceId(string priceId)
    {
        var plan = _subscriptionPlanRepository.GetSubscriptionPlanByPriceId(priceId);
        if (plan == null)
        {
            return 0;
        }
        return plan.Id;
    }

    public ResponseBase<Account> CreateConnectAccount(User currentUser)
    {
        try
        {
            var options = new AccountCreateOptions
            {
                // Type = "custom",
                Type = "express",

                Email = currentUser.Email,
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions
                    {
                        Requested = true,
                    },
                    Transfers = new AccountCapabilitiesTransfersOptions
                    {
                        Requested = true,
                    },
                },

                // ExternalAccount = "tok_mastercard_debit_transferSuccesssssss",
            };
            var service = new AccountService();
            var account = service.Create(options);
            return ResponseBase<Account>.Success(account);
        }
        catch (Exception)
        {
            return ResponseBase<Account>.Failure(CustomResponseStatus.FailedToCreateConnectAccount);
        }
    }

    public new ResponseBase<CustomerDto> CreateCustomer(CustomerInput customerInput)
    {
        try
        {
            return base.CreateCustomer(customerInput);
        }
        catch (Exception)
        {
            return ResponseBase<CustomerDto>.Failure(CustomResponseStatus.FailedToCreateCustomer);
        }
    }
    public (ResponseBase<Account>, string) OnboardUserInStripeConnect(string returnUrl, string refreshUrl)
    {
        try
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return (ResponseBase<Account>.Failure(ResponseStatus.UserNotFound), "");

            if (string.IsNullOrEmpty(currentUser.StripeAccountId))
            {
                _ = _httpContextAccessor.HttpContext!.Request;
                AccountCreateOptions options1 = new AccountCreateOptions
                {
                    Type = "express"
                };
                Account account1 = new AccountService().Create(options1);
                _httpContextAccessor.HttpContext!.Session.SetString("account_id", account1.Id);
                AccountLinkCreateOptions options2 = new AccountLinkCreateOptions
                {
                    Account = account1.Id,
                    RefreshUrl = refreshUrl,
                    ReturnUrl = returnUrl,
                    Type = "account_onboarding"
                };
                AccountLink accountLink1 = new AccountLinkService().Create(options2);
                return (ResponseBase<Account>.Success(account1), accountLink1.Url);
            }

            var hasStripeAccountResult = HasStripeAccount(currentUser.StripeAccountId);
            if (hasStripeAccountResult.Status == ResponseStatus.Success && hasStripeAccountResult.Result)
            {
                return (ResponseBase<Account>.Failure(CustomResponseStatus.StripeAccountAlreadyExist), "");
            }

            var service = new AccountService();
            var account = service.Get(currentUser.StripeAccountId);

            var options = new AccountLinkCreateOptions
            {
                Account = account.Id,
                RefreshUrl = refreshUrl,
                ReturnUrl = returnUrl,
                Type = "account_onboarding",
            };
            var accountLinkService = new AccountLinkService();
            var accountLink = accountLinkService.Create(options);

            return (ResponseBase<Account>.Success(account), accountLink.Url);
        }
        catch
        {
            return (ResponseBase<Account>.Failure(CustomResponseStatus.FailedToOnboardingTheUser), "");
        }
    }

    public ResponseBase<bool> Withdrawal(double tokenCount)
    {
        var user = GetCurrentUser();
        if (user == null)
            return ResponseStatus.NotFound;

        #region validation for transfer balance

        var hastStripeAccount = HasStripeAccount(user.StripeAccountId);
        if (!hastStripeAccount.Result)
            return ResponseStatus.StripeAccountNotExist;

        var tokenPrice = Convert.ToDouble(_configuration.GetSection("TokenPrice").Value);
        var totalPrice = tokenPrice * tokenCount;

        var hastbalance = HasEnoughBalanceForPlatform(totalPrice);
        if (!hastbalance.Result)
            return CustomResponseStatus.PlatFormDontHaveEnoughBalanceInStripAccount;

        if (totalPrice * 100 < 0.50 * 100)
            return CustomResponseStatus.InValidAmountForStripePayment;

        if (totalPrice * 100 > 99999999900)
            return CustomResponseStatus.InValidAmountForStripePayment;

        var res = TransferMoney(tokenCount, totalPrice);
        if (res.Status != ResponseStatus.Success)
            return res.Status;

        var payment = new Payment()
        {
            Amount = totalPrice,
            CreatedAt = DateTime.UtcNow,
            TransferId = res.Result.Id,
            UserId = user.Id,
            PaymentConfirmationStatus = PaymentConfirmationStatus.Successful,
        };

        _paymentRepository.Add(payment);
        #endregion

        return true;
    }

    private ResponseBase<Transfer> TransferMoney(double tokenCount, double totalPrice)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.UserNotFound;

        try
        {
            var connectUser = GetConnectedUser(currentUser.StripeAccountId);
            if (connectUser.Result.Capabilities.LegacyPayments != "active" /*,"inactive","pending"*/ && connectUser.Result.Capabilities.Transfers != "active")
                return ResponseStatus.AccountNeedsToHaveTransferEnabled;

            var options = new TransferCreateOptions
            {
                Amount = (long)(totalPrice * 100),
                Currency = "usd",
                Destination = currentUser.StripeAccountId,
                //TransferGroup = "ORDER_" + order.Id,
                Metadata = new Dictionary<string, string>()
                {
                    { "userId", currentUser?.Id.ToString() },
                    ////{ "paymentId", depositPayment?.Id.ToString() },
                    //{ "paymentType",  PaymentType.WithdrawalMoney.ToString() },
                    ////{ "historyId", tokenHistory.Id.ToString() },
                    //{ "tokenCount", tokenCount.ToString() },
                },
            };
            var service = new TransferService();
            var transfer = service.Create(options);

            if (!string.IsNullOrEmpty(transfer.Id))
            {
                return ResponseBase<Transfer>.Success(transfer);
            }

            return ResponseStatus.PaymentFailed;
        }
        catch (Exception)
        {
            return ResponseStatus.PaymentFailed;
        }
    }

    public ResponseBase<BuyTokenDto> BuyToken(double tokenCount)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.UserNotFound;

        var tokenPrice = Convert.ToDouble(_configuration.GetSection("TokenPrice").Value);
        var totalPrice = tokenPrice * tokenCount;

        var depositPayment = new Payment()
        {
            Amount = totalPrice,
            CreatedAt = DateTime.UtcNow,

            // PaymentIntentId = charge.PaymentIntentId,
            // StripeChargeOrTransferId = id,
            //PaymentType = PaymentType.DepositMoney,
            //Token = tokenCount,
            UserId = currentUser.Id,
            PaymentConfirmationStatus = PaymentConfirmationStatus.PenddingConfirmation
        };

        _paymentRepository.Add(depositPayment);

        try
        {
            // Create a PaymentIntent:
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalPrice * 100),
                Currency = "usd",

                // TransferGroup = poNumber,
                ReceiptEmail = currentUser.Email,

                Metadata = new Dictionary<string, string>()
                {
                    { "userId", currentUser?.Id.ToString() },
                    { "paymentId", depositPayment?.Id.ToString() },
                    //{ "paymentType", depositPayment.PaymentType.ToString() },
                    //{ "historyId", tokenHistory.Id.ToString() },

                    { "tokenCount", tokenCount.ToString() },
                }
            };

            var paymentIntentService = new PaymentIntentService();

            PaymentIntent paymentIntent;
            try
            {
                paymentIntent = paymentIntentService.Create(paymentIntentOptions);
            }
            catch (Exception)
            {
                RemovePayment(depositPayment);
                return ResponseStatus.PaymentFailed;
            }
            return new BuyTokenDto(paymentIntent.ClientSecret, totalPrice);
        }
        catch (Exception)
        {
            RemovePayment(depositPayment);
            return ResponseStatus.PaymentFailed;
        }
    }

    private void RemovePayment(Payment payment)
    {
        try
        {
            _paymentRepository.Remove(payment);
        }
        catch (Exception)
        {
        }
    }

    public async ValueTask UpdatePaymentConfirmationStatus(int paymentId, string paymentIntentId, PaymentConfirmationStatus paymentConfirmationStatus)
    {
        //var payment = _paymentRepository.GetById<Payment>(paymentId, false);
        var payment = _paymentRepository.GetPayment(paymentId, false);
        if (payment != null)
        {
            payment.PaymentConfirmationStatus = paymentConfirmationStatus;
            payment.PaymentIntentId = paymentIntentId;
            await _paymentRepository.UpdateAsync(payment);
        }
    }

    public async Task ConfirmPostPayment(
        int postId,
        int adsId,
        string paymentIntentId,
        int paymentId,
        PaymentConfirmationStatus paymentConfirmationStatus,
        PaymentStatus paymentStatus)
    {
        //var post = _paymentRepository.Find<Post>(postId);
        var post = _postRepository.GetPostById(postId).FirstOrDefault();
        if (post == null)
            return;

        //var ads = _paymentRepository.Find<Ads>(adsId);
        var ads = _adRepository.GetAdById(adsId).FirstOrDefault();
        if (ads.PostId != post.Id)
            return;
        if (ads.IsCompletedPayment)
            return;
        ads.IsCompletedPayment = true;
        ads.IsActive = true;
        ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);
        var initialPrice = Convert.ToDouble(_configuration.GetSection("InitialPrice").Value);
        ads.PricePerPerson = initialPrice;

        //post.PromoteOrAdsPriceScore = ads.NumberOfPeopleCanSee;
        if (paymentStatus == PaymentStatus.PayForPromotePost)
        {
            post.IsPromote = true;
            post.LatestPromoteDate = DateTime.UtcNow;

            //var settings = _paymentRepository.GetDbSet<Settings>().FirstOrDefault();
            var settings = _settingsRepository.GetFirstSettings();
            if (settings != null)
            {
                settings.TotalPromotedPostsCount++;
                _paymentRepository.Update(settings);
            }
        }

        if (paymentStatus == PaymentStatus.PayForPostAds)
        {
            //var settings = _paymentRepository.GetDbSet<Settings>().FirstOrDefault();
            var settings = _settingsRepository.GetFirstSettings();
            if (settings != null)
            {
                settings.TotalAdsesCount++;
                _paymentRepository.Update(settings);
            }
        }
        //var payment = await _paymentRepository.Where<Payment>(c => c.Id == paymentId).FirstAsync();
        var payment = await _paymentRepository.GetPayment(paymentId);
        if (payment != null)
        {
            payment.PaymentConfirmationStatus = paymentConfirmationStatus;
            payment.PaymentIntentId = paymentIntentId;
        }

        try
        {
            await _paymentRepository.BeginTransactionAsync();

            _paymentRepository.Update(ads);
            _paymentRepository.Update(post);
            _paymentRepository.Update(payment);

            _paymentRepository.CommitTransaction();
        }
        catch
        {
            _paymentRepository.RollBackTransaction();
        }
    }

    public async Task FailPostPayment(
        int postId,
        int adsId,
        string paymentIntentId,
        int paymentId,
        PaymentConfirmationStatus paymentConfirmationStatus,
        PaymentStatus paymentStatus)
    {
		//var post = _paymentRepository.Find<Post>(postId);
		var post = _postRepository.GetPostById(postId).FirstOrDefault();
        if (post == null)
            return;

        //var ads = _paymentRepository.Find<Ads>(adsId);
        var ads = _adRepository.GetAdById(adsId).FirstOrDefault();
        if (ads.PostId != post.Id)
            return;
        if (ads.IsCompletedPayment)
            return;

        //var payment = await _paymentRepository.Where<Payment>(c => c.Id == paymentId).FirstAsync();
        var payment = await _paymentRepository.GetPayment(paymentId);
        if (payment == null)
            return;
        if (payment.PaymentConfirmationStatus != PaymentConfirmationStatus.PenddingConfirmation)
            return;

        payment.PaymentConfirmationStatus = paymentConfirmationStatus;
        payment.PaymentIntentId = paymentIntentId;
        _paymentRepository.Update(payment);
    }

    public async Task ConfirmArticlePayment(
        int articleId,
        int adsId,
        string paymentIntentId,
        int paymentId,
        PaymentConfirmationStatus paymentConfirmationStatus,
        PaymentStatus paymentStatus)
    {
        //var article = _paymentRepository.Find<Article>(articleId);
        var article = _articleRepository.GetArticleById(articleId).FirstOrDefault();
        if (article == null)
            return;

        //var ads = _paymentRepository.Find<Ads>(adsId);
        var ads = _adRepository.GetAdById(adsId).FirstOrDefault();
        if (ads.ArticleId != article.Id)
            return;
        if (ads.IsCompletedPayment)
            return;
        ads.IsCompletedPayment = true;
        ads.IsActive = true;
        ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);
        var initialPrice = Convert.ToDouble(_configuration.GetSection("InitialPrice").Value);
        ads.PricePerPerson = initialPrice;

        //article.PromoteOrAdsPriceScore = ads.NumberOfPeopleCanSee;
        if (paymentStatus == PaymentStatus.PayForPromoteArticle)
        {
            article.IsPromote = true;
            article.LatestPromoteDate = DateTime.UtcNow;

            //var settings = _paymentRepository.GetDbSet<Settings>().FirstOrDefault();
            var settings = _settingsRepository.GetFirstSettings();
            if (settings != null)
            {
                settings.TotalPromotedArticlesCount++;
                _paymentRepository.Update(settings);
            }
        }


        //var payment = await _paymentRepository.Where<Payment>(c => c.Id == paymentId).FirstAsync();
        var payment = await _paymentRepository.GetPayment(paymentId);
        if (payment != null)
        {
            payment.PaymentConfirmationStatus = paymentConfirmationStatus;
            payment.PaymentIntentId = paymentIntentId;
        }

        try
        {
            _paymentRepository.BeginTransaction();

            _paymentRepository.Update(ads);
            _paymentRepository.Update(article);
            _paymentRepository.Update(payment);

            _paymentRepository.CommitTransaction();
        }
        catch
        {
            _paymentRepository.RollBackTransaction();
        }
    }

    public async Task FailArticlePayment(
        int articleId,
        int adsId,
        string paymentIntentId,
        int paymentId,
        PaymentConfirmationStatus paymentConfirmationStatus,
        PaymentStatus paymentStatus)
    {
        //var article = _paymentRepository.Find<Article>(articleId);
        var article = _articleRepository.GetArticleById(articleId).FirstOrDefault();
        if (article == null)
            return;

        //var ads = _paymentRepository.Find<Ads>(adsId);
        var ads = _adRepository.GetAdById(adsId).FirstOrDefault();
        if (ads.PostId != article.Id)
            return;
        if (ads.IsCompletedPayment)
            return;

        //var payment = await _paymentRepository.Where<Payment>(c => c.Id == paymentId).FirstAsync();
        var payment = await _paymentRepository.GetPayment(paymentId);
        if (payment == null)
            return;
        if (payment.PaymentConfirmationStatus != PaymentConfirmationStatus.PenddingConfirmation)
            return;

        payment.PaymentConfirmationStatus = paymentConfirmationStatus;
        payment.PaymentIntentId = paymentIntentId;
        _paymentRepository.Update(payment);
    }

    //public ResponseBase<string> ConfirmAdsPayment(int adsId)
    //{
    //    var currentUser = GetCurrentUser();
    //    if (currentUser == null)
    //        return ResponseStatus.UserNotFound;

    //    var userFromDb = repository.Find<User>(currentUser.Id);
    //    if (currentUser == null)
    //        return ResponseStatus.UserNotFound;

    //    List<Expression<Func<Ads, object>>> includes = new()
    //    {
    //        p => p.Post,
    //    };

    //    var ads = repository.GetById(adsId, includes).FirstOrDefault();
    //    if (ads is null)
    //        return ResponseStatus.NotFound;
    //    if (ads.IsCompletedPayment)
    //        return CustomResponseStatus.AdsHasAlreadyComletedPayment;
    //    if (ads.IsActive)
    //        return CustomResponseStatus.TheAdHasAlreadyBeenActivated;

    //    if (ads.Post.PosterId != currentUser.Id)
    //        return ResponseStatus.NotAllowd;

    //    if (userFromDb.IsDeletedAccount)
    //        return ResponseStatus.AlreadyRemoved;


    //    //      "InitialPrice": "500",
    //    //"NumberOfPeoplerPerUnit": "1500"

    //    var initialPrice = Convert.ToDouble(_configuration.GetSection("InitialPrice").Value);
    //    var numberOfPeoplerPerUnit = Convert.ToDouble(_configuration.GetSection("NumberOfPeoplePerUnit").Value);
    //    var adsCost = initialPrice * ads.NumberOfPeopleCanSee / numberOfPeoplerPerUnit;

    //    if (adsCost < 1 || adsCost * 100 > 99999999900)
    //        return CustomResponseStatus.InValidAmountForStripePayment;

    //    var paymentStatus = ads.Post.PostType == PostType.Promote ? PaymentStatus.PayForPromote : PaymentStatus.PayForPostAds;
    //    var payment = new Payment()
    //    {
    //        Amount = adsCost,
    //        PaymentStatus = paymentStatus,
    //        UserId = currentUser.Id,
    //        CreatedAt = DateTime.UtcNow,
    //        CreatedDate = DateTime.UtcNow,
    //        AdsId = ads.Id,
    //        PaymentConfirmationStatus = PaymentConfirmationStatus.PenddingConfirmation,
    //    };





    //    repository.Add(payment);
    //    try
    //    {
    //        var paymentIntentOptions = new PaymentIntentCreateOptions
    //        {
    //            Customer = currentUser.StripeCustomerId,
    //            Amount = (long)(adsCost * 100),
    //            Currency = "usd",
    //            ReceiptEmail = currentUser.Email,
    //            Metadata = new Dictionary<string, string>()
    //                {
    //                    { "userId", currentUser?.Id.ToString() },
    //                    { "postId", ads.PostId.ToString()  },
    //                    { "adsId", ads.Id.ToString()  },
    //                    { "paymentId", payment?.Id.ToString() },
    //                    { "paymentStatus", paymentStatus.ToString() },
    //                }
    //        };
    //        var paymentIntentService = new PaymentIntentService();
    //        PaymentIntent paymentIntent;
    //        try
    //        {
    //            paymentIntent = paymentIntentService.Create(paymentIntentOptions);

    //            payment.PaymentIntentId = paymentIntent.Id;
    //            repository.Update(payment);
    //        }
    //        catch (Exception)
    //        {
    //            RemovePayment(payment);
    //            return ResponseStatus.PaymentFailed;
    //        }

    //        ads.LatestPaymentIntentId = paymentIntent.Id;
    //        ads.LatestPaymentDateTime = DateTime.UtcNow;

    //        repository.Update(ads);
    //        return paymentIntent.ClientSecret;
    //    }
    //    catch (Exception)
    //    {
    //        RemovePayment(payment);
    //        return ResponseStatus.PaymentFailed;
    //    }
    //}

    public ResponseBase<PaymentIntent> CreatePaymentIntent(PaymentIntentInput input)
    {
        var paymentIntentOptions = new PaymentIntentCreateOptions
        {
            Customer = input.Customer,
            Amount = (long)(input.Amount * 100),
            Currency = input.Currency,
            ReceiptEmail = input.ReceiptEmail,
            Metadata = input.MetaData,
        };
        var paymentIntentService = new PaymentIntentService();
        PaymentIntent paymentIntent;
        try
        {
            paymentIntent = paymentIntentService.Create(paymentIntentOptions);

            return paymentIntent;

            //payment.PaymentIntentId = paymentIntent.Id;
            //repository.Update(payment);

        }
        catch (Exception)
        {
            return ResponseStatus.PaymentFailed;
        }

    }

    public ResponseBase<EphemeralKeyDto> CreateEphemeralKey()
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.UserNotFound;

        var cutomerId = currentUser.StripeCustomerId;
        if (string.IsNullOrEmpty(cutomerId))
            return ResponseStatus.NotEnoghData;

        var ephemeralKeyService = new EphemeralKeyService();
        var ephemeralKeyCreateOptions = new EphemeralKeyCreateOptions()
        {
            Customer = currentUser.StripeCustomerId,
            StripeVersion = _configuration.GetSection("StripeVersion").Value,
        };

        var ephemeralKey = ephemeralKeyService.Create(ephemeralKeyCreateOptions);

        return ephemeralKey.Adapt<EphemeralKeyDto>();
    }

    public async Task<ResponseBase<string>> CreateStripeSubscription(int subscriptionPlanId, EnvironmentType env)
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        if (user.StripeCustomerId == null)
        {
            if (user.Email == null)
                return ResponseStatus.NotEnoghData;

            var customerResult = CreateCustomer(new CustomerInput
            {
                Email = user.Email,
                Name = user.DisplayName
            });

            if (!customerResult.Status)
                return ResponseStatus.Failed;

            user.StripeCustomerId = customerResult.Result.Id;
            _userRepository.Update(user);
        }

        var baseUrl = _configuration.GetSection("BaseUrl").Value;

        // Find selected package by client
        var subscriptionPlan = await _subscriptionPlanRepository.GetById(subscriptionPlanId).SingleOrDefaultAsync();
        if (subscriptionPlan == null)
        {
            return ResponseStatus.NotFound;
        }

        var options = new SessionCreateOptions
        {
            Customer = user.StripeCustomerId,
            SuccessUrl = baseUrl + "/propagator/Setting/subscription/success?env=" + (int)env + "&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = baseUrl + $"/propagator/Setting/subscription/checkout?env={(int)env}",
            Mode = "subscription",

            Metadata = new Dictionary<string, string>()
               {
                   { "userId", user.Id.ToString() },
                   { "subscriptionPlanId", subscriptionPlanId.ToString() },
                   { "paymentStatus", PaymentStatus.PayForSubscription.ToString() },
                   { "env",  Environment.GetEnvironmentVariable("env")},
               },

            SubscriptionData = new SessionSubscriptionDataOptions()
            {
                Metadata = new Dictionary<string, string>()
               {
                   { "userId", user.Id.ToString() },
                   { "subscriptionPlanId", subscriptionPlanId.ToString() },
                   { "paymentStatus", PaymentStatus.PayForSubscription.ToString() },
                   { "env",  Environment.GetEnvironmentVariable("env")},
               }
            },

            LineItems = new List<SessionLineItemOptions>
             {
               new() {
                 Price = subscriptionPlan.PriceId,
                 Quantity = 1,
               },
             },
        };

        try
        {
            // Create subscription
            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return ResponseBase<string>.Success(session.Url);
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }

    public async Task<ResponseBase<Stripe.Subscription>> CancelSubscription()
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        var service = new SubscriptionService();
        var result = await service.CancelAsync(user.SubscriptionId);
        return result;
    }

    private User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

    public async Task<ResponseBase<string>> CheckoutSessionAsync(string priceId, string baseUrl, string userId, string customerId = null)
    {
        int subscriptionPlanId = GetSubscriptionPlanIdByPriceId(priceId);
        var response = await CreateCheckoutSessionAsync(priceId, baseUrl, userId, subscriptionPlanId, customerId);

        return response;
    }

    //public int GetSubscriptionPlanIdByPriceId(string priceId)
    //{
    //    var plan = _subscriptionPlanRepository.GetSubscriptionPlanByPriceId(priceId);
    //    if (plan == null)
    //    {
    //        return 0;
    //    }
    //    return plan.Id;
    //}
}
