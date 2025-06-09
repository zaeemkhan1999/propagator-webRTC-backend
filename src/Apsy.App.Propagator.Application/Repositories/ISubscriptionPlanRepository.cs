namespace Apsy.App.Propagator.Application.Repositories;

public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
{

    SubscriptionPlan GetSubscriptionPlanByPriceId(string priceId);
}