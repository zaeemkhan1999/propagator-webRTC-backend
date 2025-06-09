namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record FollowRequestEvent(int UserFollowerId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}