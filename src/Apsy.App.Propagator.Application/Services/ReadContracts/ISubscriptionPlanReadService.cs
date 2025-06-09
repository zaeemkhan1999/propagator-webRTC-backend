namespace Apsy.App.Propagator.Application.Services.ReadContracts;

public interface ISubscriptionPlanReadService : IServiceBase<SubscriptionPlan, SubscriptionPlanInput>
{
    Task<ListResponseBase<SubscriptionPlanDto>> GetSubscriptionPlansInDtoAsync();
    int GetSubscriptionPlanIdByPriceId(string priceId);
}