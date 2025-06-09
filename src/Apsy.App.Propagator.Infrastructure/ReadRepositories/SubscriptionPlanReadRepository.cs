


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SubscriptionReadPlanRepository : Repository<SubscriptionPlan, DataWriteContext>, ISubscriptionPlanReadRepository
{
    public SubscriptionReadPlanRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
        : base(dbContextFactory)
    {
    }

    public SubscriptionPlan GetSubscriptionPlanByPriceId(string priceId)
    {
        return Context.SubscriptionPlan.FirstOrDefault(x => x.PriceId == priceId);
    }
}
