namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IUserSearchAccountRepository
 : IRepository<UserSearchAccount>
{

    #region functions
    IQueryable<UserSearchAccount> GetUserSearchAccount();
    IQueryable<User> GetUser();
#endregion
}
