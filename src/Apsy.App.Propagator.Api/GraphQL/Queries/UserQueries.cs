using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    [GraphQLName("user_getUser")]
    public SingleResponseBase<User> Get(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service(ServiceKind.Default)] IUserService service,
                     int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("user_getUsers")]
    public ListResponseBase<User> GetItems(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] IUserService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }

    [GraphQLName("user_getSingleUserInfo")]
    public ResponseBase<SingleUserDto> GetSingleUserInfo(
           [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
           [Service(ServiceKind.Default)] IUserReadService service,
           int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetSingleUserDto(userId);
    }

    [GraphQLName("user_usernameExist")]
    public ResponseBase<bool> UsernameExist([Service(ServiceKind.Default)] IUserReadService service, string username)
    {
        return service.UsernameExist(null, username);
    }

    [GraphQLName("user_signIn")]
    public SingleResponseBase<User> SignIn(ClaimsPrincipal claims)
    {
        ResponseBase<User> responseBase = Aps.CommonBack.Base.Utils.IsAuthenticated<User>(claims);
        return responseBase.Status;
    }

    [GraphQLName("user_getCurrentUser")]
    public ResponseBase<CurrentUserDto> GetCurrentUser(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IUserReadService service)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetCurrentUserFromDb();
    }
    //ResponseBase<SingleUserDto> GetUserByIdDto(int userId,int currentUserId);
    [GraphQLName("user_getUserByIdDto")]
    public ResponseBase<SingleUserDto> GetUserByIdDto(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IUserReadService service, int userId, int currentUserId)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetUserByIdDto(userId, currentUserId);
    }

    [GraphQLName("user_getSuspiciousActivities")]
    public async Task<ListResponseBase<UserSusciopiousDto>> GetSusciopiousUser(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service] IUserReadService service)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await Task.Run(()=>(  service.GetUserLogin()));
    }

    [GraphQLName("user_isTwoFacorEnabled")]
    public ResponseBase<bool> IsTwoFacorEnabled(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IUserReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.IsTwoFacorEnabled();
    }

    [GraphQLName("user_getAdminInfo")]
    public ResponseBase<AdminInfoDto> GetAdminInfo([Service] IUserReadService service)
    {
        return service.GetAdminInfo();
    }

    [GraphQLName("user_getUserById")]
    public ResponseBase<User> GetUserById([Service] IUserReadService service, int userId)
    {
        return service.GetUserById(userId);
    }

    [GraphQLName("user_getUsersHaveNotBlocked")]
    public async Task<ListResponseBase<User>> GetUsersHaveNotBlocked([Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication, [Service] IUserReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.GetListUsersHaveNotBlocked();
    }

    [GraphQLName("user_getResetPasswordRequests")]
    public ListResponseBase<ResetPasswordRequest> GetResetPasswordRequests([Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IUserReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.GetResetPasswordRequests();
    }
}