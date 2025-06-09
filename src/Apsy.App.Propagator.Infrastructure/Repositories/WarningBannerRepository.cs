namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class WarningBannerRepository : Repository<WarningBanner, DataReadContext>, IWarningBannerRepository
{
    public WarningBannerRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;

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
