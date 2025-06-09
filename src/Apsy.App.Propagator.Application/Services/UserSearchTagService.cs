namespace Apsy.App.Propagator.Application.Services;

public class UserSearchTagService : ServiceBase<UserSearchTag, UserSearchTagInput>, IUserSearchTagService
{
    public UserSearchTagService(IUserSearchTagRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private IUserSearchTagRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public override ListResponseBase<UserSearchTag> Get(Expression<Func<UserSearchTag, bool>> predicate = null, bool checkDeleted = false)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var result = repository.GetUserSearchTag().Where(c => c.UserId == currentUser.Id);

        return new(result);
    }

    public override ResponseBase<UserSearchTag> Add(UserSearchTagInput input)
    {
        if (!repository.GetUser().Any(a => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        var userSearchedArticle = repository.GetUserSearchTag().Where(a => a.Tag == input.Tag && a.UserId == input.UserId).FirstOrDefault();
        if (userSearchedArticle != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newUserSearchTag = new UserSearchTag { UserId = (int)input.UserId, Tag = input.Tag };
        return repository.Add(newUserSearchTag);
    }

    public ResponseBase<UserSearchTag> DeleteSearchedTag(int userId, string tag)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchedTag = repository
                                    .GetUserSearchTag().Where(a => a.Tag == tag && a.UserId == userId)
                                    .FirstOrDefault();
        if (userSearchedTag == null)
            return ResponseStatus.NotFound;

        if (userSearchedTag.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userSearchedTag);

        return userSearchedTag;
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
