

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class NotInterestedArticleReadRepository : Repository< NotInterestedArticle, DataWriteContext>, INotInterestedArticleReadRepository
{
    public NotInterestedArticleReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataWriteContext context;
}