


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SubscriptionPlanRepository : Repository<SubscriptionPlan, DataReadContext>, ISubscriptionPlanRepository
{
    public SubscriptionPlanRepository(IDbContextFactory<DataReadContext> dbContextFactory)
        : base(dbContextFactory)
    {
    }

    public SubscriptionPlan GetSubscriptionPlanByPriceId(string priceId)
    {
        return Context.SubscriptionPlan.FirstOrDefault(x => x.PriceId == priceId);
    }
}
