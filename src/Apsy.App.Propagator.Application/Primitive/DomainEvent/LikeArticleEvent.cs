namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record LikeArticleEvent(int ArticleLikeId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}