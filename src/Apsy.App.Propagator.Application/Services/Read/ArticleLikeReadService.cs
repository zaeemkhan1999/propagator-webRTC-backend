namespace Apsy.App.Propagator.Application.Services.Read;

public class ArticleLikeReadService : ServiceBase<ArticleLike, ArticleLikeInput>, IArticleLikeReadService
{
    public ArticleLikeReadService(IArticleLikeReadRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IArticleLikeReadRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ListResponseBase<ArticleLikeDto> GetArticleLikes(User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        return (ListResponseBase<ArticleLikeDto>)repository.GetArticleLikes(currentUser);
    }
}
