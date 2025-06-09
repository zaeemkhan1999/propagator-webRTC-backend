using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class TagQueries
{
    [GraphQLName("tag_getTag")]
    public SingleResponseBase<Tag> Get(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ITagService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("tag_getTags")]
    public ExtendedListResponseBase<Tag> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ITagService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get().ToExtendedListResponseBase();
    }
}