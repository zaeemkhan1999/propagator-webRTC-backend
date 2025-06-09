

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchAccountReadRepository
 : Repository<UserSearchAccount,DataWriteContext>, IUserSearchAccountReadRepository
{
public UserSearchAccountReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

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
