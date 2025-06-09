

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class NotInterestedPostRepository : Repository<NotInterestedPost, DataReadContext>, INotInterestedPostRepository
{
    public NotInterestedPostRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataReadContext context;
}