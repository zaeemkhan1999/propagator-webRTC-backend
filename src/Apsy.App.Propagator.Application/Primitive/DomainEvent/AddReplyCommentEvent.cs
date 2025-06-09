namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AddReplyCommentEvent(int CommentId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}