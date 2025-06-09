using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PermissionQueries
{

    [GraphQLName("permissions_getPermissions")]
    public async Task<ListResponseBase<UserClaimsViewModel>> GetPermissions(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserReadService service, string username)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.SuperAdmin && currentUser.UserTypes != UserTypes.Admin) return ResponseStatus.NotAllowd;
        return await service.GeteUserPermission(username);
    }

    [GraphQLName("permissions_getCurrentUserPermissions")]
    public async Task<ListResponseBase<UserClaimsViewModel>> CurrentUserPermissions(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IUserReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return await service.GeteUserPermission(currentUser.Username);
    }
}