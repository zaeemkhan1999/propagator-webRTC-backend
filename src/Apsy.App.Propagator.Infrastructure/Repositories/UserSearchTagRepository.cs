

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchTagRepository
 : Repository<UserSearchTag,DataReadContext>,IUserSearchTagRepository
{
public UserSearchTagRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

#endregion
#region functions
	public IQueryable<User>GetUser()
	{
		var query = context.User.AsQueryable();
		return query;
	}
	public IQueryable<UserSearchTag> GetUserSearchTag()
	{
		var query = context.UserSearchTag.AsQueryable();
		return query;
	}
#endregion
}
