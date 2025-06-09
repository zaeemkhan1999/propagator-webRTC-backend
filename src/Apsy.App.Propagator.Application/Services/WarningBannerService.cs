namespace Apsy.App.Propagator.Application.Services;

public class WarningBannerService : ServiceBase<WarningBanner, WarningBannerInput>, IWarningBannerService
{
    public WarningBannerService(
        IWarningBannerRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
    }

    private readonly IWarningBannerRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;

    public override ResponseBase<WarningBanner> Add(WarningBannerInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;


        Post post = null;
        Article article = null;

        if (input.PostId == null && input.ArticleId == null)
        {
            var exist = repository.Any(x => x.UserId == input.UserId && x.ArticleId==null && x.PostId==null);
            if (exist)
                return CustomResponseStatus.AnActiveWarningBannerAlreadyExist;
        }
        else if (input.PostId != null)
        {
            var exist = repository.Any(x => x.UserId == input.UserId && x.PostId == input.PostId);
            if (exist)
                return CustomResponseStatus.AnActiveWarningBannerAlreadyExist;
            post = repository.GetPost().Where(c => c.Id == (int)input.PostId)
                                .Include(c => c.Poster)
                                .FirstOrDefault();
        }
        else if (input.ArticleId != null)
        {
            var exist = repository.Any(x => x.UserId == input.UserId && x.ArticleId == input.ArticleId);
            if (exist)
                return CustomResponseStatus.AnActiveWarningBannerAlreadyExist;
 
            article = repository.GetArticle().Where(c => c.Id == (int)input.ArticleId)
                           .Include(c => c.User)
                           .FirstOrDefault();
        }

        var result = base.Add(input);
        result.Result.Post = post;
        result.Result.Article = article;
        result.Result.RaiseEvent(ref _events, currentUser, CrudType.AddWarningAsBannerEvent);
        _eventStoreRepository.SaveEvents(_events);
        return result;
    }

    public override ResponseStatus SoftDelete(int entityId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        WarningBanner warningBannerbyId = repository.Where(c => c.Id == entityId)
            .Include(c => c.Post)
            .ThenInclude(c => c.Poster)
            .Include(c => c.Article)
            .ThenInclude(c => c.User)
            .FirstOrDefault();

        if (warningBannerbyId == null)
        {
            return ResponseStatus.NotFound;
        }

        if (warningBannerbyId.IsDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        var result = base.SoftDelete(entityId);
        warningBannerbyId.RaiseEvent(ref _events, currentUser, CrudType.DeleteWarningAsBannerEvent);
        _eventStoreRepository.SaveEvents(_events);

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

}