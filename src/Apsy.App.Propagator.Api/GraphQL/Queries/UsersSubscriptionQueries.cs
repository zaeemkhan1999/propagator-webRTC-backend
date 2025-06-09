using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UsersSubscriptionQueries
{
    [GraphQLDescription("Get current user subscription plan that user subscribed to.")]
    [GraphQLName("usersSubscription_getUsersSubscriptionsFeatures")]
    public async Task<ResponseBase<UsersSubscriptionsFeaturesDto>> GetUsersSubscriptionsFeatures(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUsersSubscriptionReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.GetUsersSubscriptionsFeatures();
    }
}