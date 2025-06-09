namespace Apsy.App.Propagator.Domain.Events.Events;

public class UsersSuspendedEvent : BaseEvent
{
    public UsersSuspendedEvent() : base(nameof(UsersSuspendedEvent))
    {
    }

    public int? UserId { get; set; }
    public string UserEmail { get; set; }

    public int DayCount { get; set; }
    public DateTime SuspensionLiftingDate { get; set; }
    public SuspendType SuspendType { get; set; }
    public string UserImageAddress { get; set; }
    public string UserCover { get; set; }
    public string UserDisplayName { get; set; }
    public string UserName { get; set; }
}