namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record StrikeEvent(int? PostId, int? ArticleId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}