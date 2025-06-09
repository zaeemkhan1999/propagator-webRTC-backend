namespace Apsy.App.Propagator.Domain.Events.Events;

public class AddWarningAsBannerSetEvent : BaseEvent
{
    public AddWarningAsBannerSetEvent() : base(nameof(AddWarningAsBannerSetEvent))
    {
    }

    public string Description { get; set; }

    public int UserId { get; set; }
    public string UserImageAddress { get; set; }
    public string UserCover { get; set; }
    public string UserDisplayName { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }


    public int? ArticleId { get; set; }
    public int? ArticleOwnerId { get; set; }


    public int? PostId { get; set; }
    public string YourMind { get; set; }
    public int? PostOwnerId { get; set; }
    public string PostOwnerEmail { get; set; }
}
