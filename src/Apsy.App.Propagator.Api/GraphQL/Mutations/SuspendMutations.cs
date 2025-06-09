namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class SuspendMutations
{
    [GraphQLName("suspend_suspend")]
    public ResponseBase<Suspend> SuspendUser(
             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
             [Service] ISuspendService service,
             SuspendInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.AuthenticationFailed;
        input.SuspendType = SuspendType.Reqular;
        return service.Add(input);
    }

    [GraphQLName("suspend_unSuspend")]
    public ResponseBase<User> UnSuspendUser(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] ISuspendService service,
            int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.AuthenticationFailed;

        return service.UnSuspend(userId);
    }
}