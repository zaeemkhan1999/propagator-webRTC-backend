namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AddStoryCommentEvent(int StoryCommentId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}