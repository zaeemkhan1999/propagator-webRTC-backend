namespace Apsy.App.Propagator.Application.Services;

public class ArticleService : ServiceBase<Article, ArticleInput>, IArticleService
{
    public ArticleService(
        IArticleRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IPaymentService paymentService,
        IConfiguration configuration,
        IArticleLikeRepository articleLikeRepository,
        IPublisher publisher,
        IEventStoreRepository eventStoreRepository
, ISaveArticleRepository saveArticleRepository,
        IInterestedUserService interestedUserService) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _paymentService = paymentService;
        _configuration = configuration;
        _publisher = publisher;
        _eventStoreRepository = eventStoreRepository;
        _saveArticleRepository = saveArticleRepository;
        _articleLikeRepository = articleLikeRepository;
        _events = new List<BaseEvent>();
        this.interestedUserService = interestedUserService;
    }

    private readonly IArticleRepository repository;
    private readonly ISaveArticleRepository _saveArticleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;
    private readonly IPublisher _publisher;
    private readonly IArticleLikeRepository _articleLikeRepository;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;
    private readonly IInterestedUserService interestedUserService;
    public override ResponseBase<Article> Add(ArticleInput input)
    {
        if (!repository.Ratelimiter(input.UserId, "ARTICLE"))
        {
            var config = TypeAdapterConfig<Article, ArticleInput>
                         .NewConfig()
                         .Map(dest => dest.ArticleItems, src => src.ArticleItems);

            var article = input.Adapt<Article>();
            article.ArticleItemsString = JsonConvert.SerializeObject(input.ArticleItems);

            var Links = new List<Link>();
            if (input.LinkInputs != null && input.LinkInputs.Any())
            {
                input.LinkInputs = input.LinkInputs.Distinct().ToList();
                var links = input.LinkInputs.Where(c => !repository.GetDbSet<Link>().Any(x => x.LinkType == LinkType.Article && x.Text == c.Text && x.Url == c.Url)).Distinct();
                foreach (var item in links)
                {
                    item.LinkType = LinkType.Article;
                    item.PostId = null;
                    Links.Add(item.Adapt<Link>());
                }
                repository.AddRange(Links);
            }

            article.Links = Links;
            article.LatestUpdateThisWeeks = DateTime.UtcNow;
            repository.Add(article);
            UpdateSettings(true);
            return new ResponseBase<Article>(article);
        }
        else
        {
            return CustomResponseStatus.ArticleLimit;
        }

    }

    private void UpdateSettings(bool isReqularArticle)
    {
        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            if (isReqularArticle)
                settings.TotalArticlesCount++;
            repository.Update(settings);
        }
    }

    public ResponseBase<ArticleAdsDto> PromoteArticle(PromoteArticleInput input, User currentUser)
    {
        if (!currentUser.ProfessionalAccount)
            return CustomResponseStatus.JustProfessionalAccountCandPromotThePosts;

        if (input.ManualStatus == ManualStatus.Manual && (input.TargetGenders is null
                 || input.TargetStartAge is null
                 || input.TargetEndAge is null
                 || string.IsNullOrEmpty(input.TargetLocation)))
        {
            return ResponseStatus.NotEnoghData;
        }

        if (input.IsWithOutPayment && (currentUser == null || currentUser.UserTypes == UserTypes.User))
        {
            return ResponseStatus.NotEnoghData;
        }
        if (currentUser == null && input.IsWithOutPayment)
            return ResponseStatus.NotEnoghData;

        try
        {

            Discount discount = null;
            if (!string.IsNullOrEmpty(input.DiscountCode))
            {
                if (repository.Any<UserDiscount>(d => d.UserId == currentUser.Id && d.Discount.DiscountCode == input.DiscountCode))
                    return CustomResponseStatus.DiscountAlreadyUsed;
                discount = repository.Where<Discount>(d => d.DiscountCode == input.DiscountCode).FirstOrDefault();


                if (discount is null)
                    return CustomResponseStatus.DiscountNotFound;
                if (discount.ExpireDate < DateTime.Now)
                    return CustomResponseStatus.DiscountExpire;
            }


            repository.BeginTransaction();

            var article = repository.Where(c => c.Id == input.ArticleId).FirstOrDefault();
            if (article == null)
                return ResponseStatus.NotFound;
            if (article.IsPromote)
                return CustomResponseStatus.AlreadyPromoted;

            var hasActiveAds = repository.Any(c => c.Id == input.ArticleId && c.Adses.Any(c => c.IsActive));
            if (hasActiveAds)
                return CustomResponseStatus.ThisArticleHasActiveAdsOrActivePromote;

            var ads = input.Adapt<Ads>();
            ads.IsActive = false;
            ads.IsCompletedPayment = false;
            ads.Type = AdsType.PromoteArtilce;
            ads.UserId = currentUser.Id;

            if (input.IsWithOutPayment)
            {
                ads.IsActive = true;
                ads.IsCompletedPayment = true;
                ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);
            }
            repository.Add(ads);


            if (input.IsWithOutPayment)
            {
                var user = repository.Find<User>((int)article.UserId);
                ads.Article.User = user;
                ads.RaiseEvent(ref _events, currentUser, CrudType.AdsWithOutPayment);
                _eventStoreRepository.SaveEvents(_events);
                repository.CommitTransaction();
                return new ArticleAdsDto(article, "");
            }

            var clientSecretResult = AddArtilcePromotePayment(currentUser, ref ads, discount);
            if (discount is not null)
                repository.Add<UserDiscount>(new UserDiscount { UserId = currentUser.Id, DiscountId = discount.Id });

            var result = new ArticleAdsDto(article, clientSecretResult?.Result);

            repository.CommitTransaction();
            return result;
        }
        catch
        {
            repository.RollBackTransaction();
            return ResponseStatus.Failed;
        }
    }


    private ResponseBase<string> AddArtilcePromotePayment(User currentUser, ref Ads ads, Discount discount = null)
    {
        var initialPrice = Convert.ToDouble(_configuration.GetSection("InitialPrice").Value);
        var numberOfPeoplerPerUnit = Convert.ToDouble(_configuration.GetSection("NumberOfPeoplePerUnit").Value);
        var adsCost = initialPrice * ads.NumberOfPeopleCanSee / numberOfPeoplerPerUnit;

        if (adsCost < 1 || adsCost * 100 > 99999999900)
            return CustomResponseStatus.InValidAmountForStripePayment;

        double amountdiscount = 0;

        if (discount is not null)
        {
            amountdiscount = discount.Amount > 0 ? discount.Amount : (((double)discount.Percent / (double)100) * adsCost);
            adsCost = adsCost - amountdiscount;

            if (adsCost < 0)
                adsCost = 0;
        }

        var paymentStatus = PaymentStatus.PayForPromoteArticle;

        var payment = new Payment()
        {
            Amount = adsCost,
            PaymentStatus = paymentStatus,
            AmountWithoutDiscount = adsCost + amountdiscount,

            UserId = currentUser?.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            AdsId = ads.Id,
            PaymentConfirmationStatus = PaymentConfirmationStatus.PenddingConfirmation,
        };


        if (adsCost == 0)
        {
            payment.PaymentConfirmationStatus = PaymentConfirmationStatus.Successful;
        }
        repository.Add(payment);

        var paymentIntentInput = new PaymentIntentInput(
            "usd",
            adsCost,
            currentUser.StripeCustomerId,
            currentUser.Email,
            new Dictionary<string, string>() {
                    { "userId", currentUser?.Id.ToString() },
                    { "articleId", ads.ArticleId.ToString()  },
                    { "adsId", ads.Id.ToString()  },
                    { "paymentId", payment?.Id.ToString() },
                    { "paymentStatus", paymentStatus.ToString() },
            });

        try
        {
            if (adsCost == 0)
            {

                ads.IsActive = true;
                ads.IsCompletedPayment = true;
                ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);
                repository.Update(ads);
                return "";

            }
            else
            {
                var paymentIntent = _paymentService.CreatePaymentIntent(paymentIntentInput);
                if (paymentIntent.Status != ResponseStatus.Success)
                    return paymentIntent.Status;

                payment.PaymentIntentId = paymentIntent.Result.Id;
                repository.Update(payment);

                ads.LatestPaymentIntentId = paymentIntent.Result.Id;
                ads.LatestPaymentDateTime = DateTime.UtcNow;

                repository.Update(ads);
                return paymentIntent?.Result?.ClientSecret;
            }
        }
        catch
        {
            repository.Remove(payment);
            return ResponseStatus.PaymentFailed;
        }
    }

    public ResponseStatus SoftDelete(int entityId, User currentUser)
    {

        Article article = repository.GetArticle(entityId);
        if (article == null)
            return ResponseBase<Article>.Failure(ResponseStatus.NotFound);

        if (article.DeletedBy != DeletedBy.NotDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        if (currentUser.UserTypes == UserTypes.User && article.UserId != currentUser?.Id)
            return ResponseBase<Article>.Failure(ResponseStatus.AuthenticationFailed);

        if (currentUser.UserTypes == UserTypes.User && article.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        article.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;
        var removedArtice = repository.Update(article);

        //var result = base.SoftDelete(entityId);

        if (currentUser.UserTypes != UserTypes.User && article.UserId != currentUser?.Id)
        {
            article.RaiseEvent(ref _events, currentUser, CrudType.DeleteArticle);
            _eventStoreRepository.SaveEvents(_events);
        }
        return ResponseStatus.Success;
    }

    public ResponseBase<Article> Update(ArticleInput input, User currentUser)
    {
        Article val = input.Adapt<Article>();
        Article articleFromDb = repository.GetArticle(val.Id);
        if (articleFromDb == null)
        {
            return ResponseBase<Article>.Failure(ResponseStatus.NotFound);
        }

        if (articleFromDb.UserId != currentUser.Id && !input.IsByAdmin)
            return ResponseStatus.AuthenticationFailed;

        var Links = new List<Link>();
        if (input.LinkInputs != null && input.LinkInputs.Any())
        {
            input.LinkInputs = input.LinkInputs.Distinct().ToList();
            var links = input.LinkInputs.Where(c => !repository.GetDbSet<Link>().Any(x => x.LinkType == LinkType.Article && x.Text == c.Text && x.Url == c.Url));
            foreach (var item in links)
            {
                item.LinkType = LinkType.Article;
                item.PostId = null;
                item.ArticleId = articleFromDb.Id;
                Links.Add(item.Adapt<Link>());
            }
            repository.AddRange(Links);
        }

        input.ArticleItems = input.ArticleItems.Distinct().ToList();
        TypeAdapterConfig<Article, Article>
                         .NewConfig()
                         .Ignore(dest => dest.ArticleItems)
                         .Ignore(dest => dest.UserId);
        val.Adapt(articleFromDb);
        var articleItems = input.ArticleItems.Adapt<List<ArticleItem>>();
        articleFromDb.ArticleItems = articleItems;
        articleFromDb.ArticleItemsString = JsonConvert.SerializeObject(articleItems);

        articleFromDb.IsEdited = true;
        repository.Update(articleFromDb);
        return articleFromDb;
    }

    public ResponseBase<Article> PinArticle(int articleId, bool pin, User currentUser)
    {
        var article = repository.GetArticle(articleId);
        if (article == null)
            return ResponseStatus.NotFound;

        if (article.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        if (pin && repository.Where(c => c.UserId == currentUser.Id && c.IsPin).Count() == 4)
            return CustomResponseStatus.ExceedMaximmPinCount;

        article.PinDate = DateTime.UtcNow;
        article.IsPin = pin;
        return repository.Update(article);
    }

    public async Task<ResponseBase<Article>> VerifyArticle(int articleId, bool verify, User currentUser)
    {
        var article = repository.GetArticle(articleId);

        if (article == null)
            return ResponseStatus.NotFound;

        article.IsVerifield = verify;
        var result = repository.Update(article);

        article.RaiseEvent(ref _events, currentUser, CrudType.VerifyArticle);
        await _eventStoreRepository.SaveEventsAsync(_events);
        return result;
    }

    public async Task<ResponseBase<ArticleLike>> LikeArticle(int userId, int articleId, bool isLiked, User currentUser)
    {
        var article = repository.GetArticle(articleId);
        if (article == null)
        {
            return ResponseStatus.NotFound;
        }

        if (!repository.Any((User a) => a.Id == userId))
        {
            return ResponseStatus.UserNotFound;
        }

        ArticleLike articleLike = _articleLikeRepository.GetArticleLike(articleId, userId);
        if (articleLike == null)
        {
            ArticleLike entity = new ArticleLike
            {
                UserId = userId,
                ArticleId = articleId,
                Liked = isLiked,
            };
            articleLike = repository.Add(entity);
            article.ArticleLikesCount++;
            await repository.UpdateArticlesEngagement(article);
            var settings = repository.GetDbSet<Settings>().FirstOrDefault();
            if (settings != null)
            {
                settings.TotalArticleLikesCount++;
                repository.Update(settings);
            }
        }
        else
        {
            articleLike.Liked = isLiked;
            return Update(articleLike);
        }

        try
        {
            if (article.User.LikeNotification && currentUser.Id != article.UserId)
                await _publisher.Publish(new LikeArticleEvent(articleLike.Id, currentUser.Id, article.UserId));
        }
        catch
        {
        }
        await interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = article.UserId, UserId = currentUser.Id, InterestedUserType = InterestedUserType.Article, ArticleId = article.Id });

        return articleLike;
    }

    public async Task<ResponseBase> UnLikeArticle(int userId, int articleId)
    {
        ArticleLike articleLike = _articleLikeRepository.GetArticleLike(articleId, userId);

        if (articleLike == null)
            return ResponseStatus.NotFound;

        articleLike.Article.ArticleLikesCount--;
        repository.Remove(articleLike);
        await repository.UpdateArticlesEngagement(articleLike.Article);
        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalArticleLikesCount--;
            repository.Update(settings);
        }

        return ResponseBase.Success();
    }

    public async Task<ResponseBase<UserViewArticle>> AddView(int articleId, User currentUser)
    {
        DateTime startDateTime = DateTime.Today;
        DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1);

        var article = repository.GetArticle(articleId);
        if (article == null)
            return ResponseBase<UserViewArticle>.Failure(ResponseStatus.NotFound);

        var usersViewInArticle = repository
                      .Where<UserViewArticle>(a => a.ArticleId == articleId && a.UserId == currentUser.Id && a.CreatedDate > startDateTime && a.CreatedDate < endDateTime)
                      .Count();

        var userViewArticle = new UserViewArticle()
        {
            ArticleId = articleId,
            UserId = currentUser.Id,
        };

        if (usersViewInArticle >= 1)
            return ResponseBase<UserViewArticle>.Success(userViewArticle);

        article.Hits++;
        var viewResult = repository.Add(userViewArticle);

        await repository.UpdateArticlesEngagement(article);

        if (article.ArticleType != ArticleType.RegularArticle)
        {
            var activeAds = repository.Where<Ads>(c => c.ArticleId == articleId && c.IsCompletedPayment && c.IsActive).FirstOrDefault();
            if (activeAds != null)
            {
                activeAds.TotalViewed++;
                if (activeAds.NumberOfPeopleCanSee <= activeAds.TotalViewed)
                {
                    activeAds.IsActive = false;
                    if (article.IsPromote)
                        article.IsPromote = false;
                }
                repository.Update(activeAds);
            }
        }

        return ResponseBase<UserViewArticle>.Success(viewResult);
    }
    public async Task<ResponseBase> SaveArticle(int userId, int articleId)
    {
        var article = repository.GetArticle(articleId);
        if (article == null)
        {
            return ResponseBase.Failure(ResponseStatus.NotFound);
        }

        if (!repository.Any<User>(a => a.Id == userId))
        {
            return ResponseBase.Failure(ResponseStatus.UserNotFound);
        }

        var saveArticle = _saveArticleRepository.GetSaveArticle(articleId, userId);
        if (saveArticle != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newSaveArticle = new SaveArticle { UserId = userId, ArticleId = articleId };
        article.SaveArticlesCount++;

        repository.Add(newSaveArticle);
        repository.Update(article);
        await repository.UpdateArticlesEngagement(article);
        return ResponseBase.Success();
    }

    public async Task<ResponseBase> UnSavePost(int userId, int articleId)
    {
        var saveArticle = _saveArticleRepository.GetSaveArticle(articleId, userId);
        if (saveArticle == null)
            return ResponseStatus.NotFound;

        saveArticle.Article.SaveArticlesCount--;
        repository.Remove(saveArticle);
        await repository.UpdateArticlesEngagement(saveArticle.Article);

        return ResponseBase.Success();
    }
    public async Task<ResponseStatus> UndoDeleteArticle(int entityId)
    {
        Article article = repository.GetArticle(entityId);

        if (article == null)
            return ResponseBase<Article>.Failure(ResponseStatus.NotFound);

        if (article.DeletedBy == DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyUndo;
        }

        article.DeletedBy = DeletedBy.NotDeleted;
        await repository.UpdateAsync(article);
        return ResponseStatus.Success;
    }
}
