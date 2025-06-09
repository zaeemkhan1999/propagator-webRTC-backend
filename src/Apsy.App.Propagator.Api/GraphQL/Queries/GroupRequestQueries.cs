using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
    public class GroupRequestQueries
    {
        [GraphQLName("groupRequest_PendingGroupRequests")]

        public ListResponseBase<GroupRequestDto> Get([Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication, [Service] IGroupRequestReadService service,int groupId)
        {
            if (authentication.Status != ResponseStatus.Success)
            {
                return authentication.Status;
            }
            User currentUser = authentication.CurrentUser;
            return service.GetPendingGroupRequests(currentUser.Id,groupId);
        }
    }