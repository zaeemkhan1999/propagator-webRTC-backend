namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SupportQueries
{
    [GraphQLName("support_getSupport")]
    public SingleResponseBase<Support> Get(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] ISupportService service,
            int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.Get(entityId);
    }

    [GraphQLName("support_getSupports")]
    public ListResponseBase<Support> GetItems(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] ISupportService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.Get();
    }
}