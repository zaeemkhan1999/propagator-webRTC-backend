namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserSearchAccountMutations
{
    [GraphQLName("userSearchAccount_createUserSearchAccount")]
    public ResponseBase<UserSearchAccount> Create(
             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
           [Service(ServiceKind.Default)] IUserSearchAccountService service,
           UserSearchAccountInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.SearcherId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("userSearchAccount_removeUserSearchAccount")]
    public ResponseStatus Remove(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IUserSearchAccountService service,
            int accountId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.DeleteSearchedAccount(currentUser.Id, accountId);
    }

    [GraphQLName("userSearchAccount_removeAllUserSearchAccount")]
    public async Task<ResponseBase<bool>> RemoveAll(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IUserSearchAccountService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.DeleteAllSearchedAccount();
    }
}