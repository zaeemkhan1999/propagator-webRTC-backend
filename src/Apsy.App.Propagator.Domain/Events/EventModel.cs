namespace Apsy.App.Propagator.Domain.Events;

public class EventModel : EntityDef
{
    public DateTime TimeStamp { get; set; }
    //public Guid AggregateIdentifier { get; set; }
    //public string AggregateType { get; set; }
    public int Version { get; set; }
    public string EventType { get; set; }
    public string EventData { get; set; }
    //public BaseEvent EventData { get; set; }
    public User Admin { get; set; }
    public int? AdminId { get; set; }

}