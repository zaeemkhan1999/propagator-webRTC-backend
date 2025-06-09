

namespace Apsy.App.Propagator.Application;

public interface IEventStoreRepository : IRepository<EventModel>
{
    Task SaveEventsAsync(IEnumerable<BaseEvent> events, int expectedVersion = -1);
    void SaveEvents(IEnumerable<BaseEvent> events, int expectedVersion = -1);
}