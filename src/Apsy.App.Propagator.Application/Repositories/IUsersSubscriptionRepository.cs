namespace Apsy.App.Propagator.Application.Repositories;

public interface IUsersSubscriptionRepository : IRepository<UsersSubscription>
{
    IQueryable<UsersSubscription> GetUsersSubscriptions();
    Task<User> GetUserByIdAsync(int userId);
    Task<SubscriptionPlan> GetSubscriptionPlanById(int subscriptionPlanId);
}
