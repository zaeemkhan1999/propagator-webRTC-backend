

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchPlaceRepository
 : Repository<UserSearchPlace,DataReadContext>,IUserSearchPlaceRepository
{
public UserSearchPlaceRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

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
