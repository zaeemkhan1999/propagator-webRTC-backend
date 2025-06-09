using Aps.CommonBack.Base.Extensions;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.Common.Api.Core;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;


[ExtendObjectType(typeof(Mutation))]


public class GroupRequestMutations
{
    [GraphQLName("groupRequest_RequestToJoin")]
    public ResponseBase<GroupRequest> RequestToJoinGroup(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication, [Service] IGroupRequestReadService service,
        [Service] IMessageReadService messageService,int groupId)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        
        var query = messageService.GetGroup(groupId, authentication.CurrentUser);  

        int groupAdminId = query.Result.AdminId.Value;
        GroupRequest groupRequest = new()
        {
            UserId = currentUser.Id,
            GroupId = groupId,
            GroupAdminId = groupAdminId,
            Status = "PENDING"
        };
        var checkRequest = service.GetGroupRequestEntity(currentUser.Id,groupId);

        if (checkRequest == null)
        {
            var add = service.Add(groupRequest);
            return add;
        }


        if(checkRequest.Status == "APPROVED") return ResponseStatus.NotAllowd;
        if(checkRequest.Status == "PENDING") return ResponseStatus.AlreadyExists;
        
        var append = service.Add(groupRequest);
        return append;
        
        
    }

    [GraphQLName("groupRequest_ApproveUser")]
    public ResponseBase<GroupRequest> ApproveGroupRequest(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication, 
        [Service] IGroupRequestReadService service,
        int userId, int groupId)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
         var groupRequest = service.GetGroupRequestEntity(userId, groupId);

         if (groupRequest == null)
        {
            return ResponseStatus.NotFound;
        }

        if (groupRequest.Status == "APPROVED")
        {
            return ResponseStatus.AlreadyExists;
        }

        // Update the entity status
        groupRequest.Status = "APPROVED";
        
        var update = service.Update(groupRequest);
        return update;
    }
}