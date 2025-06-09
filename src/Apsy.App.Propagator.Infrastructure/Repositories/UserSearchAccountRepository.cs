

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchAccountRepository
 : Repository<UserSearchAccount,DataReadContext>,IUserSearchAccountRepository
{
public UserSearchAccountRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

    #endregion
    #region functions
    public IQueryable<UserSearchAccount> GetUserSearchAccount()
    {
        var query = context.UserSearchAccount.AsQueryable();
        return query;
    }
    public IQueryable<User>GetUser()
    {
        var qyery = context.User.AsQueryable();
        return qyery;
    }
    #endregion
}
