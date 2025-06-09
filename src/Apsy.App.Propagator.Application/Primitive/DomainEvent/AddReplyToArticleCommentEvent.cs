namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AddReplyToArticleCommentEvent(int ArticleCommentId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}