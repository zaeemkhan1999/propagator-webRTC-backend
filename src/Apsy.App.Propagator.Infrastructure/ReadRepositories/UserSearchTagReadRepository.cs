

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchTagReadRepository
 : Repository<UserSearchTag,DataWriteContext>, IUserSearchTagReadRepository
{
public UserSearchTagReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

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
