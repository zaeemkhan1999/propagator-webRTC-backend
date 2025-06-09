namespace Apsy.App.Propagator.Domain.Events.Events;

public class AdsWithoutPaymentCreatedEvent : BaseEvent
{
    public AdsWithoutPaymentCreatedEvent() : base(nameof(AdsWithoutPaymentCreatedEvent))
    {
    }
}