namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SuspendQueries
{
    [GraphQLName("suspend_getSuspend")]
    public SingleResponseBase<Suspend> Get(
                               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                               [Service(ServiceKind.Default)] ISuspendService service,
                               int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.Get(entityId);
    }

    [GraphQLName("suspend_getSuspends")]
    public ListResponseBase<Suspend> GetItems(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] ISuspendService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(d => d.User.IsSuspended == true);
    }
}