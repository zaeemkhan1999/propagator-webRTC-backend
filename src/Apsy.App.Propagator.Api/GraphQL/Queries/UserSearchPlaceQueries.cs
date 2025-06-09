namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserSearchPlaceQueries
{
    [GraphQLName("userSearchPlace_getUserSearchPlace")]
    public SingleResponseBase<UserSearchPlace> Get(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] IUserSearchPlaceService service,
                                int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("userSearchPlace_getuserSearchPlaces")]
    public ListResponseBase<UserSearchPlace> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchPlaceService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }
}