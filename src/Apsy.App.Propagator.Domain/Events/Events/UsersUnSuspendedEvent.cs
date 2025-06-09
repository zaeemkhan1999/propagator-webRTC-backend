namespace Apsy.App.Propagator.Domain.Events.Events;


public class UsersUnSuspendedEvent : BaseEvent
{
    public UsersUnSuspendedEvent() : base(nameof(UsersUnSuspendedEvent))
    {
    }

    public int? UserId { get; set; }
    public string UserEmail { get; set; }
    public string UserImageAddress { get; set; }
    public string UserCover { get; set; }
    public string UserDisplayName { get; set; }
    public string UserName { get; set; }
}
