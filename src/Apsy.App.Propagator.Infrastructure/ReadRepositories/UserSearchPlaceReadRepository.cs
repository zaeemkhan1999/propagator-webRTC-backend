

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchPlaceReadRepository
 : Repository<UserSearchPlace,DataWriteContext>,IUserSearchPlaceReadRepository
{
public UserSearchPlaceReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

#endregion
#region functions
	public IQueryable<UserSearchPlace> GetUserSearchPlace()
	{
		var query = context.UserSearchPlace.AsQueryable();
		return query;
	}
    public IQueryable<User> GetUser()
    {
        var query = context.User.AsQueryable();
        return query;
    }

    #endregion
}
