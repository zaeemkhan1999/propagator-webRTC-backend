

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ViewArticleReadRepository
 : Repository<ViewArticle,DataWriteContext>, IViewArticleReadRepository
{
public ViewArticleReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

    #endregion
    #region functions
    public IQueryable<ViewArticle> GetAllViewArticle()
    {
        var query = context.ViewArticle.AsQueryable();
        return query;
    }
    #endregion
}
