namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record SendSecretDirectMessageEvent(SecretMessage SecretMessage, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}
