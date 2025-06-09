

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class NotInterestedPostReadRepository : Repository<NotInterestedPost, DataWriteContext>, INotInterestedPostReadRepository
{
    public NotInterestedPostReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataWriteContext context;
}