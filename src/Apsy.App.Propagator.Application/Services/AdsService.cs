namespace Apsy.App.Propagator.Application.Services;

public class AdsService : ServiceBase<Ads, AdsInput>, IAdsService
{
    public AdsService(
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

    private readonly IAdReadRepository repository;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;
    private readonly IPublisher _publisher;
    private readonly IAppealAdsRepository _appealAdsRepository;


    public SingleResponseBase<AdsDto> GetAds(int id,User currentUser)
    {

        var postQueryable = repository.GetbyId(id,currentUser.Id);
        if (postQueryable.Any())
        {
            return new SingleResponseBase<AdsDto>(postQueryable);
        }

        return ResponseStatus.NotFound;
    }

    public ListResponseBase<AdsDto> GetAdses( User currentUser)
    {
        return new ListResponseBase<AdsDto>(repository.GetbyAll(currentUser.Id));
    }

    public ListResponseBase<AdsDto> GetAdsesForSlider(List<int> ignoredAdsIds, User currentuser)
    {
        return new(repository.GetAdsesForSlider(ignoredAdsIds,currentuser));
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


    
    public override ResponseStatus SoftDelete(int entityId)
    {
        return base.SoftDelete(entityId);
    }

    public ResponseBase<Ads> SuspendAds(int adsId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var ads = repository.GetbyAdsId(adsId);
        if (ads == null)
            return ResponseStatus.NotFound;

        if (ads.AdsRejectionStatus != AdsRejectionStatus.Active)
        {
            return ResponseStatus.NotEnoghData;
        }

        ads.AdsRejectionStatus = AdsRejectionStatus.Suspended;

        var result = repository.Update(ads);

        result.RaiseEvent(ref _events, currentUser, CrudType.SuspendAds);
        _eventStoreRepository.SaveEvents(_events);

        return result;
    }

    public ResponseBase<Ads> UnSuspendAds(int adsId, User currentUser)
    {
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;


        var ads = repository.GetbyAdsId(adsId);
        if (ads == null)
            return ResponseStatus.NotFound;

        if (ads.AdsRejectionStatus != AdsRejectionStatus.Suspended)
        {
            return ResponseStatus.NotEnoghData;
        }
        ads.AdsRejectionStatus = AdsRejectionStatus.Active;
        var result = repository.Update(ads);

        result.RaiseEvent(ref _events, currentUser, CrudType.UnSuspendAds);
        _eventStoreRepository.SaveEvents(_events);

        return result;
    }

    public async Task<ResponseBase<Ads>> RejectAds(int adsId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var ads = repository.GetbyAdsId(adsId);
        if (ads == null)
            return ResponseStatus.NotFound;

        if (ads.AdsRejectionStatus != AdsRejectionStatus.Active)
        {
            return ResponseStatus.NotEnoghData;
        }

        ads.AdsRejectionStatus = AdsRejectionStatus.Rejected;

        var result = repository.Update(ads);

        var receiverId = ads.Post != null ? ads.Post.PosterId : ads.Article.UserId;

        await _publisher.Publish(new RejectAdsEvent(ads.Id, currentUser.Id, receiverId));

        result.RaiseEvent(ref _events, currentUser, CrudType.RejectAds);
        _eventStoreRepository.SaveEvents(_events);

        return result;
    }

    public ResponseBase<Ads> UnRejectAds(int adsId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;
        var ads = repository.GetbyAdsId(adsId);
        if (ads == null)
            return ResponseStatus.NotFound;

        if (ads.AdsRejectionStatus != AdsRejectionStatus.Rejected)
        {
            return ResponseStatus.NotEnoghData;
        }
        ads.AdsRejectionStatus = AdsRejectionStatus.Active;
        var result = repository.Update(ads);

        result.RaiseEvent(ref _events, currentUser, CrudType.UnRejectAds);
        _eventStoreRepository.SaveEvents(_events);

        return result;
    }

    public async Task<ResponseBase<AppealAds>> Appeal(AppealAdsInput input)
    {
        if (!repository.Any(d => d.Id == input.AdsId && d.AdsRejectionStatus == AdsRejectionStatus.Rejected))
            return ResponseStatus.NotAllowd;

        var appeal = input.Adapt<AppealAds>();
        appeal.AdminId = input.UserId;
        await _appealAdsRepository.AddAsync(appeal);

        return ResponseBase<AppealAds>.Success(appeal);
    }

    public async Task<ResponseBase> RejectAppeal(int appealAdsId, string reasonReject)
    {
        var appeal = await _appealAdsRepository.GetbyId(appealAdsId);

        if (appeal == null)
            return ResponseStatus.NotFound;

        appeal.AppealStatus = AppealStatus.Rejected;
        appeal.ReasonReject = reasonReject;
        await _appealAdsRepository.UpdateAsync(appeal);

        return ResponseBase.Success();
    }

    public async Task<ResponseBase> AcceptAppeal(int appealAdsId)
    {
        var appeal = await _appealAdsRepository.GetbyId(appealAdsId);

        if (appeal == null)
            return ResponseStatus.NotFound;

        appeal.AppealStatus = AppealStatus.Accepted;
        appeal.Ads.AdsRejectionStatus = AdsRejectionStatus.Active;
        await _appealAdsRepository.UpdateAsync(appeal);

        return ResponseBase.Success();
    }

}