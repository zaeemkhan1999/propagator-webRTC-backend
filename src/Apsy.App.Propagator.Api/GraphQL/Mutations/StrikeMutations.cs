namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class StrikeMutations
{
    [GraphQLName("strike_createStrike")]
    public ResponseBase<Strike> CreateStrike(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStrikeService service,
        StrikeInput input)
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

    [GraphQLName("strike_removeStrike")]
    public async Task<ResponseBase> RemoveStrike(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStrikeService service,
        int id)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;


        return await service.RemoveStrike(id, authentication.CurrentUser);
    }
}