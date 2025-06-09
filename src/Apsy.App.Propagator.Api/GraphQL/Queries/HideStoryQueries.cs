using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class HideStoryQueries
{

    [GraphQLName("hideStory_getMyHideStory")]
    public SingleResponseBase<HideStory> Get(
                              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service(ServiceKind.Default)] IHideStoryReadService service,
         int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.Get(entityId);
    }


    [GraphQLName("hideStory_getMyHideStories")]
    public ListResponseBase<HideStoryDto> GetItems(
                              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IHideStoryReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetHideStories(authentication.CurrentUser);
    }
}