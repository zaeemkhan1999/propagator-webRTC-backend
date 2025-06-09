namespace Apsy.App.Propagator.Domain.Events.Events;

public class ArticleCommentRemovedEvent : BaseEvent
{
    public ArticleCommentRemovedEvent() : base(nameof(ArticleCommentRemovedEvent))
    {
    }

    public int ArticleCommentId { get; set; }
    public int ArticleId { get; set; }


    public string Author { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }

    public int? ArticleOwnerId { get; set; }
    public string ArticleOwnerEmail { get; set; }
    public string CommentText { get; set; }
    public string CommentContentAddress { get; set; }
    public CommentType CommentType { get; set; }

}