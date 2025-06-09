

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UsersSubscriptionRepository : Repository<UsersSubscription, DataReadContext>, IUsersSubscriptionRepository
{
    public UsersSubscriptionRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private DataReadContext context;

    public IQueryable<UsersSubscription> GetUsersSubscriptions()
    {
        var query = context.UsersSubscription.AsQueryable();
        return query;
    }
    public async Task<User> GetUserByIdAsync(int userId)
    {
        var query = await context.User.FindAsync(userId);
        return query;
    }
    public async Task<SubscriptionPlan> GetSubscriptionPlanById(int subscriptionPlanId)
    {
        var query = await context.SubscriptionPlan.FindAsync(subscriptionPlanId);
        return query;
    }
}
