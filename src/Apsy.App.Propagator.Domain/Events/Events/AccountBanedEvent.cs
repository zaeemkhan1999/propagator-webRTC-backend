namespace Apsy.App.Propagator.Domain.Events.Events;

public class AccountBanedEvent : BaseEvent
{
    public AccountBanedEvent() : base(nameof(AccountBanedEvent))
    {
    }


    public int UserId { get; set; }

    /// <summary>
    /// if IsBaned=false means this account becom unBaned
    /// if IsBaned=true means this account becom Baned
    /// </summary>
    public bool IsBaned { get; set; }
    public string UserEmail { get; set; }
    public string UserImageAddress { get; set; }
    public string UserCover { get; set; }
    public string UserDisplayName { get; set; }
    public string UserName { get; set; }

}