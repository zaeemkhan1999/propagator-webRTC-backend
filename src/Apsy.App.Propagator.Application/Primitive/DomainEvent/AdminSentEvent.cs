namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AdminSentEvent(int? SenderId, int RecieverId, Notification Notification) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}