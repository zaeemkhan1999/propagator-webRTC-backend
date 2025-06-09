using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class AdsQueries
{
    [GraphQLName("ads_getAds")]
    public SingleResponseBase<AdsDto> GetAds(
                        [Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IAdsReadService service,
                        int entityId)
    {
        return service.GetAds(entityId,authentication.CurrentUser);
    }

    [GraphQLName("ads_getAdses")]
    public ListResponseBase<AdsDto> GetAdses(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IAdsReadService service)
    {
        return service.GetAdses(authentication.CurrentUser);
    }

    [GraphQLName("ads_getAdsesForSlider")]
    public ListResponseBase<AdsDto> GetAdsesForSlider(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IAdsReadService service,
        List<int> ignoredAdsIds)
    {
        return service.GetAdsesForSlider(ignoredAdsIds,authentication.CurrentUser);
    }


}