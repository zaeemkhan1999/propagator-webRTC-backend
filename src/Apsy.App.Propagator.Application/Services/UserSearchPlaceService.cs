namespace Apsy.App.Propagator.Application.Services;

public class UserSearchPlaceService : ServiceBase<UserSearchPlace, UserSearchPlaceInput>, IUserSearchPlaceService
{
    public UserSearchPlaceService(IUserSearchPlaceRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IUserSearchPlaceRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public override ListResponseBase<UserSearchPlace> Get(Expression<Func<UserSearchPlace, bool>> predicate = null, bool checkDeleted = false)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var result = repository.GetUserSearchPlace().Where(c => c.UserId == currentUser.Id);

        return new(result);
    }

    public override ResponseBase<UserSearchPlace> Add(UserSearchPlaceInput input)
    {
        if (!repository.GetUser().Any(a => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        var userSearchedArticle = repository.GetUserSearchPlace().Where(a => a.Place == input.Place && a.UserId == input.UserId).FirstOrDefault();
        if (userSearchedArticle != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newUserSearchPlace = new UserSearchPlace { UserId = (int)input.UserId, Place = input.Place };
        return repository.Add(newUserSearchPlace);
    }

    public ResponseBase<UserSearchPlace> DeleteSearchedPlace(int userId, string place)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchedPlace = repository
                                    .GetUserSearchPlace().Where(a => a.Place == place && a.UserId == userId)
                                    .FirstOrDefault();
        if (userSearchedPlace == null)
            return ResponseStatus.NotFound;

        if (userSearchedPlace.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userSearchedPlace);

        return userSearchedPlace;
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