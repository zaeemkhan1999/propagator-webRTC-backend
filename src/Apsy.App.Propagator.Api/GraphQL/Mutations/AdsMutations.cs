namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class AdsMutations
{
    [GraphQLName("ads_deleteAds")]
    public ResponseBase<Ads> DeleteAds(
                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                             [Service] IAdsService service,
                             int adsId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.SoftDelete(adsId);
    }

    [GraphQLName("ads_suspendAds")]
    public ResponseBase<Ads> SuspendAds(
                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                             [Service] IAdsService service,
                             int adsId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.SuspendAds(adsId,currentUser);
    }

    [GraphQLName("ads_unSuspendAds")]
    public ResponseBase<Ads> UnSuspendAds(
                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                             [Service] IAdsService service,
                             int adsId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.UnSuspendAds(adsId,currentUser);
    }

    [GraphQLName("ads_rejectAds")]
    public async Task<ResponseBase<Ads>> RejectAds(
                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                             [Service] IAdsService service,
                             int adsId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.RejectAds(adsId,currentUser);
    }

    [GraphQLName("ads_unRejectAds")]
    public ResponseBase<Ads> UnRejectAds(
                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                             [Service] IAdsService service,
                             int adsId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.UnRejectAds(adsId,currentUser);
    }


    [GraphQLName("ads_appealAds")]
    public async Task<ResponseBase<AppealAds>> AppealAds(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service] IAdsService service,
                     AppealAdsInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin && currentUser.UserTypes != UserTypes.User) return ResponseStatus.NotAllowd;
        input.UserId = currentUser.Id;

        return await service.Appeal(input);
    }

    [GraphQLName("ads_appealReject")]
    public async Task<ResponseBase> AppealRejectAds(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service] IAdsService service,
                     int appealAdsId, string reasonReject)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.RejectAppeal(appealAdsId, reasonReject);
    }


    [GraphQLName("ads_appealAccept")]
    public async Task<ResponseBase> AppealAccept(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service] IAdsService service,
                     int appealAdsId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.AcceptAppeal(appealAdsId);
    }
}