using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;


[ExtendObjectType(typeof(Query))]
    public class GroupTopicQueries
    {
        [GraphQLName("groupTopic_GetGroupTopic")]
        public SingleResponseBase<GroupTopicDto> Get(
             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
             [Service(ServiceKind.Default)] IGroupTopicReadService service,
             int entityId)
        {
            if (authentication.Status != ResponseStatus.Success)
            {
                return authentication.Status;
            }
            return service.GetGroupTopic(entityId);
        }
        [GraphQLName("groupTopic_GetGroupTopicAll")]
        public ListResponseBase<GroupTopicDto> GetAll(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
            [Service(ServiceKind.Default)] IGroupTopicReadService service
            )
        {
            if (authentication.Status != ResponseStatus.Success)
            {
                return authentication.Status;
            }
            return service.GetGroupTopic();
        }
    }

