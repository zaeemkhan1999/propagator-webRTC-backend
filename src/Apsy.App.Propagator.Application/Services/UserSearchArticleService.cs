namespace Apsy.App.Propagator.Application.Services;

public class UserSearchArticleService : ServiceBase<UserSearchArticle, UserSearchArticleInput>, IUserSearchArticleService
{
    public UserSearchArticleService(
        IUserSearchArticleRepository repository,
        IHttpContextAccessor httpContextAccessor)
    : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IUserSearchArticleRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public override ListResponseBase<UserSearchArticle> Get(Expression<Func<UserSearchArticle, bool>> predicate = null, bool checkDeleted = false)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var result = repository.GetUserSearchArticle().Where(c => c.UserId == currentUser.Id);

        return new(result);
    }

    public override ResponseBase<UserSearchArticle> Add(UserSearchArticleInput input)
    {
        if (!repository.Any<User>(a => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        var userSearchedArticle = repository.GetUserSearchArticle().Where(a => a.ArticleId == input.ArticleId && a.UserId == input.UserId).FirstOrDefault();
        if (userSearchedArticle != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newUserSearchArticle = new UserSearchArticle { UserId = (int)input.UserId, ArticleId = (int)input.ArticleId };
        return repository.Add(newUserSearchArticle);
    }

    public ResponseBase<UserSearchArticle> DeleteSearchedArticle(int userId, int articleId,User currentUser)
    {
        
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchedArticle = repository
                                    .GetUserSearchArticle().Where(a => a.ArticleId == articleId && a.UserId == userId)
                                    .FirstOrDefault();
        if (userSearchedArticle == null)
            return ResponseStatus.NotFound;

        if (userSearchedArticle.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userSearchedArticle);

        return userSearchedArticle;
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
