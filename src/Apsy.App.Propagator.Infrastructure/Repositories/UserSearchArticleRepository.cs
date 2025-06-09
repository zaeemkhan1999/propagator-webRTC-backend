

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchArticleRepository
 : Repository<UserSearchArticle,DataReadContext>,IUserSearchArticleRepository
{
public UserSearchArticleRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

#endregion
#region functions
	public IQueryable<UserSearchArticle> GetUserSearchArticle()
	{
		var query = context.UserSearchArticle.AsQueryable();
		return query;

    }
#endregion
}
