namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record LikeCommentEvent(int LikeCommentId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}