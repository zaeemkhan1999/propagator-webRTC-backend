using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserSearchGroupQueries
{
    [GraphQLName("userSearchGroup_getUserSearchGroup")]
    public SingleResponseBase<UserSearchGroupDto> GetUserSearchGroup(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] IUserSearchGroupReadService service,
                                int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetUserSearchGroup(entityId);
    }

    [GraphQLName("userSearchGroup_getUserSearchGroups")]
    public ListResponseBase<UserSearchGroupDto> GetUserSearchGroups(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchGroupReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetUserSearchGroups(authentication.CurrentUser);
    }

}