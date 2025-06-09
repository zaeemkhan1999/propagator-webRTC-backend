namespace Apsy.App.Propagator.Domain.Events.Events;

public class AccountDeletedEvent : BaseEvent
{
    public AccountDeletedEvent() : base(nameof(AccountDeletedEvent))
    {
    }
    public int? DeletedUserId { get; set; }
    public string DeletedUserEmail { get; set; }
    public string DeletedUserUsername { get; set; }
    public string DeletedUserPhoneNumber { get; set; }
    public string DeletedUserImageAddress { get; set; }
    public string DeletedUserCover { get; set; }
}