namespace Apsy.App.Propagator.Application.Services;

public class UserSearchAccountService : ServiceBase<UserSearchAccount, UserSearchAccountInput>, IUserSearchAccountService
{
    public UserSearchAccountService(IUserSearchAccountRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IUserSearchAccountRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public override ListResponseBase<UserSearchAccount> Get(Expression<Func<UserSearchAccount, bool>> predicate = null, bool checkDeleted = false)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var result = repository.GetUserSearchAccount().Where(c => c.SearcherId == currentUser.Id);

        return new(result);
    }

    public override ResponseBase<UserSearchAccount> Add(UserSearchAccountInput input)
    {
        if (!repository.Any<User>(a => a.DisplayName == input.SearchedName))
        {
            return ResponseStatus.UserNotFound;
        }
        else
        {
            input.SearchedId = repository.GetUser().Where(a => a.DisplayName == input.SearchedName).FirstOrDefault().Id;
        }
        var userSearchedAccount = repository.GetUserSearchAccount().Where(a => a.SearchedId == input.SearchedId && a.SearcherId == input.SearcherId).FirstOrDefault();
        if (userSearchedAccount != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newUserSearchAccount = new UserSearchAccount { SearcherId = (int)input.SearcherId, SearchedId = (int)input.SearchedId };
        return repository.Add(newUserSearchAccount);
    }

    public ResponseBase<UserSearchAccount> DeleteSearchedAccount(int searcherId, int searchedId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchedAccount = repository
                                    .GetUserSearchAccount().Where(a => a.SearchedId == searchedId && a.SearcherId == searcherId)
                                    .FirstOrDefault();
        if (userSearchedAccount == null)
            return ResponseStatus.NotFound;

        if (userSearchedAccount.SearcherId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userSearchedAccount);

        return userSearchedAccount;
    }

    public async Task<ResponseBase<bool>> DeleteAllSearchedAccount()
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchedAccounts = await repository
                                    .GetUserSearchAccount().Where(a => a.SearcherId == currentUser.Id)
                                    .ToListAsync();

        repository.RemoveRange(userSearchedAccounts);
        return true;
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