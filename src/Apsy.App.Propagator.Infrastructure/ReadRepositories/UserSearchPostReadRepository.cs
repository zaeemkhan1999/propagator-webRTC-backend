

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchPostReadRepository
 : Repository<UserSearchPost,DataWriteContext>, IUserSearchPostReadRepository
{
public UserSearchPostReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

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
