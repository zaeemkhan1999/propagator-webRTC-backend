namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record LikeStoryEvent(int StoryLikeId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}