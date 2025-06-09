using Apsy.App.Propagator.Application.Services;
using Apsy.App.Propagator.Domain.Entities;
using Mapster;
using Propagator.Common.Services.Contracts;
using Stripe;
using Stripe.Checkout;

namespace Propagator.Common.Services
{
    //public class StripeSerivceBase<TPayment, TPaymentInput, TUser, TCustomerInput, TStripeFullPayModel> : ServiceBase<TPayment, TPaymentInput>, IStripeSerivceBase<TPayment, TPaymentInput, TUser, TCustomerInput, TStripeFullPayModel>, IServiceBase<TPayment, TPaymentInput> where TPayment : EntityDef, new() where TPaymentInput : InputDef where TUser : UserDef where TCustomerInput : CustomerInputDef, new() where TStripeFullPayModel : StripFullPaymentInputDef
    public class StripeSerivceBase<TPayment, TPaymentInput, TUser, TCustomerInput, TStripeFullPayModel> : ServiceBase<TPayment, TPaymentInput>, IStripeSerivceBase<TPayment, TPaymentInput, TUser, TCustomerInput, TStripeFullPayModel>, IServiceBase<TPayment, TPaymentInput> where TPayment : EntityDef, new() where TPaymentInput : StripPaymentInput where TUser : UserDef where TCustomerInput : CustomerInput, new() where TStripeFullPayModel : StripFullPaymentInput
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IRepository<TPayment> _stripeRepositoryBase;

        private TokenService tokenService;

        private CustomerService customerService;

        private ChargeService chargeService;

        private CardService cardService;

        public StripeSerivceBase(IRepository<TPayment> stripeRepositoryBase, IHttpContextAccessor httpContextAccessor)
            : base(stripeRepositoryBase)
        {
            _stripeRepositoryBase = stripeRepositoryBase;
            _httpContextAccessor = httpContextAccessor;
            tokenService = new TokenService();
            customerService = new CustomerService();
            chargeService = new ChargeService();
            cardService = new CardService();
        }

        public virtual async Task<ResponseBase<ChargeDto>> CreateCharge(TPaymentInput paymentInput, string currency = "usd")
        {
            ChargeCreateOptions chargeCreateOptions = new ChargeCreateOptions
            {
                Amount = ((currency == "usd") ? ((long)(Convert.ToDouble(paymentInput.Amount) * 100.0)) : ((long)paymentInput.Amount)),
                Currency = currency,
                Source = paymentInput.StripeToken,
                Metadata = paymentInput.Metadata
            };
            if (paymentInput.CustomerId != null)
            {
                chargeCreateOptions.Customer = paymentInput.CustomerId;
            }

            Charge obj = await new ChargeService().CreateAsync(chargeCreateOptions);
            ChargeDto result = obj.Adapt<ChargeDto>();
            if (obj.Status == "succeeded")
            {
                return ResponseBase<ChargeDto>.Success(result);
            }

            return null;
        }

        public virtual ResponseStatus CreateChargeByFullData(TStripeFullPayModel paymentInput)
        {
            try
            {
                Customer customer = customerService.Get(paymentInput.CustomerId);
                TokenCreateOptions options = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = paymentInput.CardNumder,
                        ExpMonth = paymentInput.ExpMonth.ToString(),
                        ExpYear = paymentInput.ExpYear.ToString(),
                        Cvc = paymentInput.CVC
                    }
                };
                Token stripeToken = tokenService.Create(options);
                IEnumerable<Card> enumerable = (from s in cardService.List(paymentInput.CustomerId)?.GroupBy((Card g) => g.Fingerprint)
                                                select s.First());
                string text = ((enumerable == null) ? stripeToken.Card.Id : enumerable.FirstOrDefault((Card f) => f.Fingerprint == stripeToken.Card.Fingerprint)?.Id);
                if (string.IsNullOrEmpty(text))
                {
                    CardCreateOptions options2 = new CardCreateOptions
                    {
                        Source = stripeToken.Id
                    };
                    cardService.Create(paymentInput.CustomerId, options2);
                }

                customer.DefaultSourceId = text;
                CustomerUpdateOptions options3 = customer.Adapt<CustomerUpdateOptions>();
                customerService.Update(paymentInput.CustomerId, options3);
                ChargeCreateOptions options4 = new ChargeCreateOptions
                {
                    Amount = ((paymentInput.Currency == "usd") ? ((long)(Convert.ToDouble(paymentInput.Amount) * 100.0)) : paymentInput.Amount),
                    Currency = paymentInput.Currency,
                    Description = paymentInput.Description,
                    Metadata = paymentInput.Metadata,
                    Customer = paymentInput.CustomerId
                };
                if (!chargeService.Create(options4).Paid)
                {
                    return ResponseStatus.PaymentFailed;
                }

                return ResponseStatus.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual ResponseBase<CustomerDto> CreateCustomer(TCustomerInput customerInput)
        {
            CustomerCreateOptions options = new CustomerCreateOptions
            {
                Email = customerInput.Email,
                Name = customerInput.Name,
                Balance = customerInput.Balance,
                Description = customerInput.Description,
                Metadata = customerInput.Metadata
            };
            return ResponseBase<CustomerDto>.Success(new CustomerService().Create(options).Adapt<CustomerDto>());
        }

        public virtual ResponseStatus SubscribeToPlan(string planId, string customerId)
        {
            SubscriptionCreateOptions options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    }
                }
            };
            if (!string.IsNullOrEmpty(new SubscriptionService().Create(options).Id))
            {
                return ResponseStatus.Success;
            }

            return ResponseStatus.Failed;
        }

        public virtual async Task<ResponseBase<string>> CreateCheckoutSessionAsync(string priceId, string baseUrl, string userId, int subscriptionPlanId, string customerId = null)
        {
            //below code is commented because it is not getting Environment variable value, it reutns null
            //string envValue = "devTest";
            //if (Environment.GetEnvironmentVariable("env") != null && Environment.GetEnvironmentVariable("env") != "")
            //{
            //    envValue = Environment.GetEnvironmentVariable("env");
            //}

            SessionCreateOptions options = new SessionCreateOptions
            {
                Customer = customerId,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1L,
                    }
                },
                Mode = "subscription",
                SuccessUrl = baseUrl + "/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = baseUrl,
                Metadata = new Dictionary<string, string>()
                           {
                               { "userId",userId },
                                { "subscriptionPlanId", subscriptionPlanId.ToString() },
                              // { "srvName",  envValue},
                           },

                SubscriptionData = new SessionSubscriptionDataOptions()
                {
                    Metadata = new Dictionary<string, string>()
                               {
                                   { "userId", userId },
                                   { "subscriptionPlanId", subscriptionPlanId.ToString() },
                                 //  { "srvName", envValue},
                               }
                },

            };
            try
            {
                return (await new SessionService().CreateAsync(options)).Id;
            }
            catch
            {
                return ResponseBase<string>.Failure(ResponseStatus.Failed);
            }
        }

        public virtual ResponseStatus IsTransferEnabled(string userStripeAccountId)
        {
            if (string.IsNullOrEmpty(userStripeAccountId))
            {
                return ResponseBase<bool>.Failure(ResponseStatus.StripeAccountNotExist);
            }

            ResponseBase<Account> connectedUser = GetConnectedUser(userStripeAccountId);
            if (connectedUser.Status != ResponseStatus.Success)
            {
                return ResponseBase<bool>.Failure(connectedUser.Status);
            }

            if (connectedUser == null || connectedUser.Result.Capabilities == null)
            {
                return ResponseStatus.Failed;
            }

            if (connectedUser.Result.Capabilities.Transfers != "active")
            {
                return ResponseStatus.Failed;
            }

            return ResponseStatus.Success;
        }

        public virtual ResponseBase<bool> HasStripeAccount(string userStripeAccountId)
        {
            if (string.IsNullOrEmpty(userStripeAccountId))
            {
                return ResponseBase<bool>.Success(result: false);
            }

            ResponseBase<Account> connectedUser = GetConnectedUser(userStripeAccountId);
            if (connectedUser.Status != ResponseStatus.Success)
            {
                return false;
            }

            if (!connectedUser.Result.DetailsSubmitted || !connectedUser.Result.ChargesEnabled)
            {
                return false;
            }

            return true;
        }

        public virtual ResponseBase<bool> HasEnoughBalanceForConnectUser(double amount, string userStripeAccountId)
        {
            if (string.IsNullOrEmpty(userStripeAccountId))
            {
                return false;
            }

            if (GetConnectedUser(userStripeAccountId).Status != ResponseStatus.Success)
            {
                return false;
            }

            return ResponseBase<bool>.Success((double)GetConnectUserBlance(userStripeAccountId).Result > amount * 100.0 + 1.0);
        }

        public virtual ResponseBase<bool> HasEnoughBalanceForPlatform(double amount)
        {
            return ResponseBase<bool>.Success((double)GetPlatformBlance().Result > amount * 100.0 + 1.0);
        }

        public virtual ResponseBase<long> GetConnectUserBlance(string userStripeAccountId)
        {
            if (string.IsNullOrEmpty(userStripeAccountId))
            {
                return 0L;
            }

            RequestOptions requestOptions = new RequestOptions();
            requestOptions.StripeAccount = userStripeAccountId;
            return new BalanceService().Get(requestOptions).Available.Sum((BalanceAmount c) => c.Amount);
        }

        public virtual ResponseBase<long> GetPlatformBlance()
        {
            return ResponseBase<long>.Success(new BalanceService().Get().Available.Sum((BalanceAmount c) => c.Amount));
        }

        public virtual ResponseBase<TransferDto> TransferMoneyToAccount(string userStripeAccountId, double cost, Dictionary<string, string> metadata, string currency = "usd", string transferGroup = null)
        {
            ResponseBase<Account> connectedUser = GetConnectedUser(userStripeAccountId);
            if (connectedUser.Result.Capabilities.LegacyPayments != "active" && connectedUser.Result.Capabilities.Transfers != "active")
            {
                return ResponseBase<TransferDto>.Failure(ResponseStatus.AccountNeedsToHaveTransferEnabled);
            }

            TransferCreateOptions options = new TransferCreateOptions
            {
                Amount = ((currency == "usd") ? ((long)(cost * 100.0)) : ((long)cost)),
                Currency = currency,
                Destination = userStripeAccountId,
                TransferGroup = transferGroup,
                Metadata = metadata
            };
            Transfer transfer = new TransferService().Create(options);
            if (!string.IsNullOrEmpty(transfer.Id))
            {
                return ResponseBase<TransferDto>.Success(transfer.Adapt<TransferDto>());
            }

            return ResponseBase<TransferDto>.Failure(ResponseStatus.PaymentFailed);
        }

        public virtual ResponseBase<ChargeDto> GetMoneyFromConnectAccount(string userStripeAccountId, double cost, Dictionary<string, string> metadata, string currency = "usd")
        {
            ChargeCreateOptions options = new ChargeCreateOptions
            {
                Amount = ((currency == "usd") ? ((long)(cost * 100.0)) : ((long)cost)),
                Currency = currency,
                Source = userStripeAccountId,
                Metadata = metadata
            };
            Charge charge = new ChargeService().Create(options);
            if (charge.Status == "succeeded")
            {
                return ResponseBase<ChargeDto>.Success(charge.Adapt<ChargeDto>());
            }

            return ResponseBase<ChargeDto>.Failure(ResponseStatus.FailedToWidthraw);
        }

        public virtual ListResponseBase<Charge> GeCharges()
        {
            return ListResponseBase<Charge>.Success(new ChargeService().List().ToList().AsQueryable());
        }

        public virtual ListResponseBase<ChargeDto> GeChargesData()
        {
            return ListResponseBase<ChargeDto>.Success(new ChargeService().List().ToList().AsQueryable()
                .ProjectToType<ChargeDto>());
        }

        public virtual ResponseBase<Balance> GeBalance(string stripeConnectedUserId = null)
        {
            if (!string.IsNullOrEmpty(stripeConnectedUserId))
            {
                RequestOptions requestOptions = new RequestOptions();
                requestOptions.StripeAccount = stripeConnectedUserId;
                return ResponseBase<Balance>.Success(new BalanceService().Get(requestOptions));
            }

            return ResponseBase<Balance>.Success(new BalanceService().Get());
        }

        public virtual ResponseBase<BalanceDto> GeBalanceData(string stripeConnectedUserId = null)
        {
            if (!string.IsNullOrEmpty(stripeConnectedUserId))
            {
                RequestOptions requestOptions = new RequestOptions();
                requestOptions.StripeAccount = stripeConnectedUserId;
                return ResponseBase<BalanceDto>.Success(new BalanceService().Get(requestOptions).Adapt<BalanceDto>());
            }

            return ResponseBase<BalanceDto>.Success(new BalanceService().Get().Adapt<BalanceDto>());
        }

        public virtual ListResponseBase<Customer> GeCustomers()
        {
            return ListResponseBase<Customer>.Success(new CustomerService().List().ToList().AsQueryable());
        }

        public virtual ListResponseBase<CustomerDto> GeCustomersData()
        {
            return ListResponseBase<CustomerDto>.Success(new CustomerService().List().ToList().AsQueryable()
                .ProjectToType<CustomerDto>());
        }

        public virtual ListResponseBase<BalanceTransaction> GeTransactions()
        {
            return ListResponseBase<BalanceTransaction>.Success(new BalanceTransactionService().List().ToList().AsQueryable());
        }

        public virtual ListResponseBase<BalanceTransactionDto> GeTransactionsData()
        {
            return ListResponseBase<BalanceTransactionDto>.Success(new BalanceTransactionService().List().ToList().AsQueryable()
                .ProjectToType<BalanceTransactionDto>());
        }

        public virtual ListResponseBase<Dispute> GeDisputes()
        {
            return ListResponseBase<Dispute>.Success(new DisputeService().List().ToList().AsQueryable());
        }

        public virtual ListResponseBase<DisputeDto> GeDisputesData()
        {
            return ListResponseBase<DisputeDto>.Success(new DisputeService().List().ToList().AsQueryable()
                .ProjectToType<DisputeDto>());
        }

        public virtual ListResponseBase<Refund> GeRefunds()
        {
            return ListResponseBase<Refund>.Success(new RefundService().List().ToList().AsQueryable());
        }

        public virtual ListResponseBase<RefundDto> GeRefundsData()
        {
            return ListResponseBase<RefundDto>.Success(new RefundService().List().ToList().AsQueryable()
                .ProjectToType<RefundDto>());
        }

        public virtual ListResponseBase<Plan> GePlans()
        {
            return ListResponseBase<Plan>.Success(new PlanService().List().ToList().AsQueryable());
        }

        public virtual ListResponseBase<PlanDto> GePlansData()
        {
            return ListResponseBase<PlanDto>.Success(new PlanService().List().ToList().AsQueryable()
                .ProjectToType<PlanDto>());
        }

        public virtual ResponseBase<Account> GetConnectedUser(string stripeConnectedUserId)
        {
            return ResponseBase<Account>.Success(new AccountService().Get(stripeConnectedUserId));
        }

        public virtual ResponseBase<AccountDto> GetConnectedUserData(string stripeConnectedUserId)
        {
            return ResponseBase<AccountDto>.Success(new AccountService().Get(stripeConnectedUserId).Adapt<AccountDto>());
        }


    }
}
