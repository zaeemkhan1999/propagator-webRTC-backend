namespace Apsy.App.Propagator.Application.Services;

public class UserSearchPostService : ServiceBase<UserSearchPost, UserSearchPostInput>, IUserSearchPostService
{
    public UserSearchPostService(IUserSearchPostRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private IUserSearchPostRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public override ListResponseBase<UserSearchPost> Get(Expression<Func<UserSearchPost, bool>> predicate = null, bool checkDeleted = false)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var result = repository.GetUserSearchPost().Where(c => c.UserId == currentUser.Id);

        return new(result);
    }

    public override ResponseBase<UserSearchPost> Add(UserSearchPostInput input)
    {
        if (!repository.GetUser().Any(a => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        var userUserSearchPost = repository.GetUserSearchPost().Where(a => a.PostId == input.PostId && a.UserId == input.UserId).FirstOrDefault();
        if (userUserSearchPost != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newUserSearchPost = new UserSearchPost { UserId = (int)input.UserId, PostId = (int)input.PostId };
        return repository.Add(newUserSearchPost);
    }

    public ResponseBase<UserSearchPost> DeleteSearchedPost(int userId, int postId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchPost = repository
                                    .GetUserSearchPost().Where(a => a.PostId == postId && a.UserId == userId)
                                    .FirstOrDefault();
        if (userSearchPost == null)
            return ResponseStatus.NotFound;

        if (userSearchPost.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userSearchPost);

        return userSearchPost;
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
