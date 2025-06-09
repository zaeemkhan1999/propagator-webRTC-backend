namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISubscriptionPlanReadRepository : IRepository<SubscriptionPlan>
{

    SubscriptionPlan GetSubscriptionPlanByPriceId(string priceId);
}