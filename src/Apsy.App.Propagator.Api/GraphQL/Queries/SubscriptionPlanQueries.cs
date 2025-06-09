using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SubscriptionPlanQueries
{
    [GraphQLName("subscriptionPlan_getSubscriptionPlans")]
    public async Task<ListResponseBase<SubscriptionPlanDto>> GetSubscriptionPlans(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISubscriptionPlanReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.GetSubscriptionPlansInDtoAsync();
    }
}
