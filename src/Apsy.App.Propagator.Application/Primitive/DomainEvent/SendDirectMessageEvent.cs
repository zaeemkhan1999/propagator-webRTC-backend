namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record SendDirectMessageEvent(Message Message, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}
