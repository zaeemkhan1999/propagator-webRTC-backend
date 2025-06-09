namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record SupsendEvent(int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}