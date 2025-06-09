namespace Apsy.App.Propagator.Domain.Events.Events;

public class AdsWithOutPaymentAddedEvent : BaseEvent
{
    public AdsWithOutPaymentAddedEvent() : base(nameof(AdsWithOutPaymentAddedEvent))
    {
    }

    public int? AdsId { get; set; }
    public AdsType AdsType { get; set; }
    public string YourMind { get; set; }

    public int? PostOwnerId { get; set; }
    public string PostOwnerEmail { get; set; }

    public AdsRejectionStatus AdsRejectionStatus { get; set; }

    public int? PostId { get; set; }
    public int? ArticleId { get; set; }
    public string PostItemsString { get; set; }
    public string ArticleItemsString { get; set; }
}