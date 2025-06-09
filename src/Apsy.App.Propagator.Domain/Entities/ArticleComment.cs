namespace Apsy.App.Propagator.Domain.Entities;

public class ArticleComment : UserKindDef<User>
{
    public string Text { get; set; }
    public CommentType CommentType { get; set; }
    public string ContentAddress { get; set; }
    public bool IsEdited { get; set; }
    public DeletedBy DeletedBy { get; set; }
    public Article Article { get; set; }
    public int ArticleId { get; set; }

    public int? ParentId { get; set; }
    public ArticleComment Parent { get; set; }
    public int? MentionId { get; set; }

    public User Mention { get; set; }
    public int LikeCount { get; set; }

    public ICollection<ArticleComment> Children { get; set; }
    public List<LikeArticleComment> LikeArticleComments { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public List<Report> Reports { get; set; }

    [GraphQLIgnore]
    public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser)
    {
        var articleCommentRemovedEvent = new ArticleCommentRemovedEvent()
        {
            AdminId = currrentUser.Id,
            ArticleCommentId = Id,
            ArticleOwnerEmail = Article.User.Email,
            ArticleOwnerId = Article?.UserId,
            ArticleId = ArticleId,
            SubTitle = Article?.SubTitle,
            Author = Article?.Author,
            Title = Article?.Title,
            CommentText = Text,
            CommentContentAddress = ContentAddress,
            CommentType = CommentType

        };

        events.Add(articleCommentRemovedEvent);
        return events;
    }
}