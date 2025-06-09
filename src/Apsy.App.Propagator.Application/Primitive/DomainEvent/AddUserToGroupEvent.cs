namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record AddUserToGroupEvent(int conversationId, bool isForAdmin, int? SenderId, int RecieverId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}