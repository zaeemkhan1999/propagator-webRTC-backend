namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public sealed record LikePostEvent(int PostLikeId, int? SenderId, int RecieverId,int PostId) : IDomainEvent
{
    public string GraphQlIgnore()
    {
        return "GraphQlIgnore";
    }
}