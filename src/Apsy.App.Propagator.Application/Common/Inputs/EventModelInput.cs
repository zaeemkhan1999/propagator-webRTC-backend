namespace Apsy.App.Propagator.Application.Common.Inputs;

public class EventModelInput : BaseInputDef
{
    public DateTime TimeStamp { get; set; }
    public Guid AggregateIdentifier { get; set; }
    public string AggregateType { get; set; }
    public int Version { get; set; }
    public string EventType { get; set; }

    [GraphQLIgnore]
    public BaseEvent EventData { get; set; }
}