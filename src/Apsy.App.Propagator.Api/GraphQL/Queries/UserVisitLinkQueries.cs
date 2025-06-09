using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserVisitLinkQueries
{
    [GraphQLName("userVisitLink_getUserVisitLink")]
    public SingleResponseBase<UserVisitLink> Get(
                                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                    [Service(ServiceKind.Default)] IUserVisitLinkService service,
                                    int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("userVisitLink_getUserVisitLinks")]
    public ListResponseBase<UserVisitLink> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserVisitLinkReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.UserVisitLinks();
    }
}