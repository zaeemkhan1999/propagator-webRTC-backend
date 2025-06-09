using Apsy.App.Propagator.Application.Services;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserSearchGroupMutations
{
    [GraphQLName("userSearchGroup_createUserSearchGroup")]
    public ResponseBase<UserSearchGroup> Create(
          [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
          [Service(ServiceKind.Default)] IUserSearchGroupService service,
          UserSearchGroupInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("userSearchGroup_removeUserSearchGroup")]
    public ResponseStatus Remove(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] UserSearchGroupService service,
            int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return service.DeleteSearchedGroup(currentUser.Id, conversationId,authentication.CurrentUser);
    }

}