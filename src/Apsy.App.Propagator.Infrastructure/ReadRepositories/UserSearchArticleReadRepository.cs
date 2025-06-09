

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchArticleReadRepository
 : Repository<UserSearchArticle,DataWriteContext>, IUserSearchArticleReadRepository
{
public UserSearchArticleReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

#endregion
#region functions
	public IQueryable<UserSearchArticle> GetUserSearchArticle()
	{
		var query = context.UserSearchArticle.AsQueryable();
		return query;

    }
#endregion
}
