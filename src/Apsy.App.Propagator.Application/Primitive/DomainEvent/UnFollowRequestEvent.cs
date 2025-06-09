namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record UnFollowRequestEvent(int UserFollowerId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}