using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace Propagator.Api.GraphQL.Subscriptions; 

[ExtendObjectType(typeof(Subscription))]
public class NotificationSubscriptions
{
    public ValueTask<ISourceStream<Notification>> SubscribeToNotificationAdded(int userId, ITopicEventReceiver receiver)
    {
        var topic = $"{userId}_NotificationAdded";
        return receiver.SubscribeAsync<Notification>(topic);
    }

    [Subscribe(With = nameof(SubscribeToNotificationAdded))]
    public Notification NotificationAdded(int userId, [EventMessage] Notification notification) => notification;
}