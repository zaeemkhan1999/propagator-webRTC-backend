using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class StrikeQueries
{
    [GraphQLName("strike_getStrike")]
    public SingleResponseBase<Strike> GetStrike(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStrikeService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.Get(entityId);
    }

    [GraphQLName("strike_getStrikes")]
    public ListResponseBase<Strike> GetStrikes(
    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
    [Service(ServiceKind.Default)] IStrikeService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }


    [GraphQLName("strike_getUserWithStrikes")]
    public ListResponseBase<StrikeDto> GetUserWithStrikes(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStrikeReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetStrikes();
    }


}