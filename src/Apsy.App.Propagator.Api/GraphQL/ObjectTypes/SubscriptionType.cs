using Subscription = Propagator.Api.GraphQL.Subscriptions.Subscription;

namespace Propagator.Api.GraphQL.ObjectTypes;

public class SubscriptionType : ObjectType<Subscription>
{
    protected override void Configure(IObjectTypeDescriptor<Subscription> descriptor)
    {
        base.Configure(descriptor);
    }
}