namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class WarningBannerMutations
{
    [GraphQLName("warningBanner_createWarningBanner")]
    public ResponseBase<WarningBanner> CreateWarningBanner(
                                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                    [Service(ServiceKind.Default)] IWarningBannerService service,
                                    WarningBannerInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        if (input.UserId == currentUser.Id)
            return ResponseStatus.NotAllowd;

        return service.Add(input);
    }

    [GraphQLName("warningBanner_removeWarningBanner")]
    public virtual ResponseStatus RemoveWarningBanner(
                          [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                          [Service(ServiceKind.Default)] IWarningBannerService service,
                          int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.SoftDelete(entityId);
    }
}