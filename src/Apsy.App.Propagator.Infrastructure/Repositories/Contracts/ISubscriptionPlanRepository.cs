namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
{

    SubscriptionPlan GetSubscriptionPlanByPriceId(string priceId);
}