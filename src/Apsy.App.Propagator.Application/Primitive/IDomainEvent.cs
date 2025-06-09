namespace Apsy.App.Propagator.Application.Primitive;

public interface IDomainEvent : INotification
{
    public string GraphQlIgnore();
}