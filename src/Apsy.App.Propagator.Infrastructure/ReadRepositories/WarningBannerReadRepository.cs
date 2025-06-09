namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class WarningBannerReadRepository : Repository<WarningBanner, DataWriteContext>, IWarningBannerReadRepository
{
    public WarningBannerReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;

    public IQueryable<Article> GetArticle()
    {
        var query = context.Article.AsQueryable();
        return query;
    }
    public IQueryable<Post> GetPost()
    {
        var query = context.Post.AsQueryable();
        return query;
    }

}
