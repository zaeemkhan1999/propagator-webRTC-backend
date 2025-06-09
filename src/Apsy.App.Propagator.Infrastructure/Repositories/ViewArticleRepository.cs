

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ViewArticleRepository
 : Repository<ViewArticle,DataReadContext>,IViewArticleRepository
{
public ViewArticleRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

    #endregion
    #region functions
    public IQueryable<ViewArticle> GetAllViewArticle()
    {
        var query = context.ViewArticle.AsQueryable();
        return query;
    }
    #endregion
}
