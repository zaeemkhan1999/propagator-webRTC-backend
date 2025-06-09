namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserSearchTagQueries
{

    [GraphQLName("userSearchTag_getUserSearchTag")]
    public SingleResponseBase<UserSearchTag> Get(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                 [Service(ServiceKind.Default)] IUserSearchTagService service,
                                 int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("userSearchTag_getUserSearchTags")]
    public ListResponseBase<UserSearchTag> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchTagService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }
}