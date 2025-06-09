namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUsersSubscriptionReadRepository : IRepository<UsersSubscription>
{
    IQueryable<UsersSubscription> GetUsersSubscriptions();
    Task<User> GetUserByIdAsync(int userId);
    Task<SubscriptionPlan> GetSubscriptionPlanById(int subscriptionPlanId);
}
