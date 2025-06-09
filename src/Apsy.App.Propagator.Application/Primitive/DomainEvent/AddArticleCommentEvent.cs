namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AddArticleCommentEvent(int ArticleCommentId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}