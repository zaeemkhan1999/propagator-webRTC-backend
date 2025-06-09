namespace Apsy.App.Propagator.Domain.Events.Events;

public class CommentRemovedEvent : BaseEvent
{
    public CommentRemovedEvent() : base(nameof(CommentRemovedEvent))
    {
    }

    public int CommentId { get; set; }
    public int PostId { get; set; }


    public string YourMind { get; set; }

    public int? PostOwnerId { get; set; }
    public string PostOwnerEmail { get; set; }
    public string CommentText { get; set; }
    public string CommentContentAddress { get; set; }
    public CommentType CommentType { get; set; }
}
