

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchPostRepository
 : Repository<UserSearchPost,DataReadContext>,IUserSearchPostRepository
{
public UserSearchPostRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

#endregion
#region functions

	public IQueryable<UserSearchPost> GetUserSearchPost()
	{
		var query = context.UserSearchPost.AsQueryable();
		return query;
	}
	public IQueryable<User>GetUser()
	{
		var query = context.User.AsQueryable();
		return query;
	}
#endregion
}
