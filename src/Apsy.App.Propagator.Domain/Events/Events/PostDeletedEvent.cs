namespace Apsy.App.Propagator.Domain.Events.Events;

public class PostDeletedEvent : BaseEvent
{
    public PostDeletedEvent() : base(nameof(PostDeletedEvent))
    {
    }
    public int? PostId { get; set; }
    public string YourMind { get; set; }
    public int? PostOwnerId { get; set; }
    public string PostOwnerEmail { get; set; }
    public string PostItemsString { get; set; }
}
