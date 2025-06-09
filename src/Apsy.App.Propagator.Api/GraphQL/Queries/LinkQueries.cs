using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class LinkQueries
{
    [GraphQLName("link_getLink")]
    public SingleResponseBase<Link> Get(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ILinkReadService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("link_getLinks")]
    public async Task<ListResponseBase<Link>> GetItemsAsync(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ILinkReadService service,
        string searchTerm = null)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.Get(searchTerm, authentication.CurrentUser.Id);
    }
}