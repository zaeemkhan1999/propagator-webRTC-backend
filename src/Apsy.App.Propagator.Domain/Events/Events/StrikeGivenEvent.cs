namespace Apsy.App.Propagator.Domain.Events.Events;

public class StrikeGivenEvent : BaseEvent
{
    public StrikeGivenEvent() : base(nameof(StrikeGivenEvent))
    {
    }
    public string Text { get; set; }
    public int? UserId { get; set; }
    public string UserEmail { get; set; }
    public string UserImageAddress { get; set; }
    public string UserCover { get; set; }
    public string UserDisplayName { get; set; }
    public string UserName { get; set; }

    public int? PostId { get; set; }
    public string YourMind { get; set; }
    public int? PostOwnerId { get; set; }

    public int? ArticleId { get; set; }
    public int? ArticleOwnerId { get; set; }
}