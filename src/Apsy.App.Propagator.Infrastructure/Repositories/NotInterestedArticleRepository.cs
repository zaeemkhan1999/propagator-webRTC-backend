

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class NotInterestedArticleRepository : Repository< NotInterestedArticle, DataReadContext>, INotInterestedArticleRepository
{
    public NotInterestedArticleRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataReadContext context;
}