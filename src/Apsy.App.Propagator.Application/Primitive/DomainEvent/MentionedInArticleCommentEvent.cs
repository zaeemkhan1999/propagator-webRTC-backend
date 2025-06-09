namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record MentionedInArticleCommentEvent(ArticleComment ArticleComment, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}