using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class StorySeenQueries
{
    [GraphQLName("storySeen_getStorySeen")]
    public virtual SingleResponseBase<StorySeen> GetStorySeen(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service(ServiceKind.Default)] IStorySeenService service,
         int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("storySeen_getStorySeens")]
    public virtual ListResponseBase<StorySeenDto> GetStorySeens(
           [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
           [Service(ServiceKind.Default)] IStorySeenReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetStorySeens();
    }
}