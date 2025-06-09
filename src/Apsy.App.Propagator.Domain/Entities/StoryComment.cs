namespace Apsy.App.Propagator.Domain.Entities;

public class StoryComment : UserKindDef<User>
{
    public string Text { get; set; }
    public Story Story { get; set; }
    public int StoryId { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}