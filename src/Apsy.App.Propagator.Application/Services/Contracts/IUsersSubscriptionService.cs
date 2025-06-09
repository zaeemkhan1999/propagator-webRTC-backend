

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUsersSubscriptionService : IServiceBase<UsersSubscription, UsersSubscriptionInput>
{
    Task<ResponseBase> ChargeUserSubscriptionPlanAsync(int userId,
        int subscriptionPlanId);
}