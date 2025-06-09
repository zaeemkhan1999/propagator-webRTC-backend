namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record MentionedInPostEvent(Post Post, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}