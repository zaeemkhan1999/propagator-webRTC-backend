namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserSearchAccountQueries
{
    [GraphQLName("userSearchAccount_getUserSearchAccount")]
    public SingleResponseBase<UserSearchAccount> Get(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                 [Service(ServiceKind.Default)] IUserSearchAccountService service,
                                 int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("userSearchAccount_getUserSearchAccounts")]
    public ListResponseBase<UserSearchAccount> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchAccountService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }
}