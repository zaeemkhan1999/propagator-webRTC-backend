namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record RejectAdsEvent(int AdsId, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}