namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record LikeArticleCommentEvent(int LikeArticleCommentId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}