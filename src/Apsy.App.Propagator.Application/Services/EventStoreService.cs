using Propagator.Common.Services.Contracts;
using User = Apsy.App.Propagator.Domain.Entities.User;

namespace Apsy.App.Propagator.Application.Services;
public class EventStoreService : ServiceBase<EventModel, EventModelInput>, IEventStoreService
{
    public EventStoreService(
        IEventStoreRepository repository,
        IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IEventStoreRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

}