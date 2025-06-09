namespace Apsy.App.Propagator.Domain.Events.Events;

public class ArticleVerifiedEvent : BaseEvent
{
    public ArticleVerifiedEvent() : base(nameof(ArticleVerifiedEvent))
    {
    }

    public int ArticleId { get; set; }
    public int ArticleOwnerId { get; set; }
    public string ArticleItemsString { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
}