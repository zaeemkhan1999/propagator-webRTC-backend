using Apsy.App.Propagator.Application;
using Google.Api.Gax;
namespace Apsy.App.Propagator.Infrastructure.Repositories
{
    public class EventStoreReadRepository : Repository<EventModel, DataWriteContext>, IEventStoreReadRepository
    {
        public EventStoreReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
        {
            context = dbContextFactory.CreateDbContext();
        }
        
        public  DataWriteContext context;
        
        public async Task SaveEventsAsync(IEnumerable<BaseEvent> events, int expectedVersion = -1)
        {
            // var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            //if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            //    throw new ConcurrencyException();

            var version = expectedVersion;
            var eventModels = new List<EventModel>();
            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                @event.Id = Guid.NewGuid(); //added by me
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    Version = version,
                    EventType = eventType,
                    EventData = JsonConvert.SerializeObject(@event),
                    //EventData = @event.ToString()
                    AdminId=@event.AdminId,
                };

                eventModels.Add(eventModel);
                //await _eventStoreRepository.SaveAsync(eventModel);
                //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                //await _eventProducer.ProduceAsync(topic, @event);
            }
            await AddRangeAsync(eventModels);
        }

        public void SaveEvents(IEnumerable<BaseEvent> events, int expectedVersion = -1)
        {
            var task = Task.Run(async () => await SaveEventsAsync(events, expectedVersion));
            task.WaitWithUnwrappedExceptions();
        }
    }
}