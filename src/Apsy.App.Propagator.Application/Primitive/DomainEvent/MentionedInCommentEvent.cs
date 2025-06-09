namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record MentionedInCommentEvent(Comment Comment, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}