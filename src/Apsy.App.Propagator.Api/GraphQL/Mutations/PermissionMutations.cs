namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class PermissionMutations
{
    [GraphQLName("permission_updatePermission")]
    public async Task<ListResponseBase<UserClaimsViewModel>> UpdatePermission(
                      [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IUserService service,
                        PermissionInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;
        return await service.UpdateUserPermission(input);
    }
}
