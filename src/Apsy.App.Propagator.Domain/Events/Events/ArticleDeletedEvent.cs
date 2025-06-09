namespace Apsy.App.Propagator.Domain.Events.Events;

public class ArticleDeletedEvent : BaseEvent
{
    public ArticleDeletedEvent() : base(nameof(ArticleDeletedEvent))
    {
    }

    public int ArticleId { get; set; }
    public int ArticleOwnerId { get; set; }
    public string ArticleOwnerEmail { get; set; }
    public string ArticleItemsString { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }

}