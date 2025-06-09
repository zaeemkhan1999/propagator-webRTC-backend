namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class HighLightQueries
{
    [GraphQLName("highLight_getHighLight")]
    public SingleResponseBase<HighLight> Get(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] IHighLightService service,
                                int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("highLight_getHighLights")]
    public ListResponseBase<HighLight> GetItems(
                                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                    [Service(ServiceKind.Default)] IHighLightService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }
}