namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AddCommentEvent(int CommentId, int? SenderId, int RecieverId, int PostId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}