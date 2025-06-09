using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace Propagator.Api.GraphQL.Subscriptions;

[ExtendObjectType(typeof(Subscription))]
public class BaseSubscriptions
{
    public ValueTask<ISourceStream<SubscriptionDto>> SubscribeToNotificationReceived(int userId, ITopicEventReceiver receiver)
    { 
        string topic = $"{userId}_Subcription";
        return receiver.SubscribeAsync<SubscriptionDto>(topic);
    }

    [Subscribe(With = nameof(SubscribeToNotificationReceived))]
    public SubscriptionDto NotificationReceived(int userId, [EventMessage] SubscriptionDto message) => message;
}