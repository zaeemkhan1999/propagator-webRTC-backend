

using Apsy.App.Propagator.Domain.Common.Dtos;

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IAdsService : IServiceBase<Ads, AdsInput>
{
    ResponseBase<Ads> SuspendAds(int adsId, User CurrentUser);
    ResponseBase<Ads> UnSuspendAds(int adsId, User CurrentUser);
    Task<ResponseBase<Ads>> RejectAds(int adsId, User CurrentUser);
    ResponseBase<Ads> UnRejectAds(int adsId, User currentUser);
    Task<ResponseBase<AppealAds>> Appeal(AppealAdsInput input);
    Task<ResponseBase> RejectAppeal(int appealAdsId, string reasonReject);
    Task<ResponseBase> AcceptAppeal(int appealAdsId);
}