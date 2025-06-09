using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace Propagator.Api.GraphQL.Subscriptions;

[ExtendObjectType(typeof(Subscription))]
public class MessageSubscription
{
    public ValueTask<ISourceStream<Message>> SubscribeToDirectMessageAdded(int userId, ITopicEventReceiver receiver)
    {
        string topic = $"{userId}_DirectMessageAdded";
        return receiver.SubscribeAsync<Message>(topic);
    }

    [Subscribe(With = nameof(SubscribeToDirectMessageAdded))]
    public Message DirectMessageAdded(int userId, [EventMessage] Message message) => message;


    public ValueTask<ISourceStream<Message>> SubscribeToDirectMessageSeened(int userId, ITopicEventReceiver receiver)
    {
        string topic = $"{userId}_DirectMessageSeened";
        return receiver.SubscribeAsync<Message>(topic);
    }

    [Subscribe(With = nameof(SubscribeToDirectMessageSeened))]
    public Message DirectMessageSeened(int userId, [EventMessage] Message message) => message;
}