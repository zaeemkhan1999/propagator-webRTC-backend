namespace Apsy.App.Propagator.Domain.Events;

public abstract class BaseEvent : Messages.Message
{
    protected BaseEvent(string type)
    {
        Type = type;

    }

    public string Type { get; set; }
    public int Version { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int AdminId { get; set; }
}