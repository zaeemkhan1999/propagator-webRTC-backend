using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class AdsReadService: ServiceBase<Ads, AdsInput>, IAdsReadService
    {
        private readonly IAdReadRepository repository;
        private readonly IEventStoreRepository _eventStoreRepository;
        private List<BaseEvent> _events;
        private readonly IPublisher _publisher;
        private IAppealAdsRepository _appealAdsRepository;
        public AdsReadService(
        IAdReadRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,
         IAppealAdsRepository appealAdsRepository,
     IPublisher publisher) : base(repository)
        {
            this.repository = repository;
            _eventStoreRepository = eventStoreRepository;
            _events = new List<BaseEvent>();
            _appealAdsRepository = appealAdsRepository;
            _publisher = publisher;
        }
        public SingleResponseBase<AdsDto> GetAds(int id, User currentUser)
        {

            var postQueryable = repository.GetbyId(id, currentUser.Id);
            if (postQueryable.Any())
            {
                return new SingleResponseBase<AdsDto>(postQueryable);
            }

            return ResponseStatus.NotFound;
        }
        public ListResponseBase<AdsDto> GetAdses(User currentUser)
        {
            return new ListResponseBase<AdsDto>(repository.GetbyAll(currentUser.Id));
        }

        public ListResponseBase<AdsDto> GetAdsesForSlider(List<int> ignoredAdsIds, User currentuser)
        {
            return new(repository.GetAdsesForSlider(ignoredAdsIds, currentuser));
        }
        private static AdsDtoStatus GetAdsDtoStatus(Ads ads)
        {
            if (ads.AdsRejectionStatus == AdsRejectionStatus.Rejected)
                return AdsDtoStatus.Rejected;
            if (ads.AdsRejectionStatus == AdsRejectionStatus.Suspended)
                return AdsDtoStatus.Suspended;
            if (ads.TotalViewed < ads.NumberOfPeopleCanSee)
                return AdsDtoStatus.Active;
            else
                return AdsDtoStatus.Complete;
        }
    }
}
