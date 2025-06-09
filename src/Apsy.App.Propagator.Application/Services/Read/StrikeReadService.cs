using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class StrikeReadService : ServiceBase<Strike, StrikeInput>, IStrikeReadService
    {
        private readonly IStrikeReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEventStoreReadRepository _eventStoreRepository;
        private List<BaseEvent> _events;
        private readonly IPublisher _publisher;
        public StrikeReadService(
       IStrikeReadRepository repository,
       IHttpContextAccessor httpContextAccessor,
       IEventStoreReadRepository eventStoreRepository,
       IPublisher publisher) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _eventStoreRepository = eventStoreRepository;
            _events = new List<BaseEvent>();
            _publisher = publisher;
        }
        public ListResponseBase<StrikeDto> GetStrikes()
        {
            var strikedUserQueryable = repository
                .GetUser()
                .Select(c => new StrikeDto()
                {
                    User = c,
                    StrikeCount = c.Strikes.Count,
                    Text = string.Join(" %% ", c.Strikes.Select(g => g.Text + " Id=" + g.Id).ToList())

                });
            return new(strikedUserQueryable);
        }
    }
}
