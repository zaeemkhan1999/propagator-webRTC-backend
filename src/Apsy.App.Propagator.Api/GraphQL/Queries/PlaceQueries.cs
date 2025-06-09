namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PlaceQueries
{
    [GraphQLName("place_getPlace")]
    public SingleResponseBase<Place> Get(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IPlaceService service,
            int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("place_getPlaces")]
    public ListResponseBase<Place> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPlaceService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.Get();
    }
}