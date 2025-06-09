namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserSearchTagMutations
{
    [GraphQLName("userSearchTag_createUserSearchTag")]
    public ResponseBase<UserSearchTag> Create(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchTagService service,
        [Service(ServiceKind.Default)] ITagService tagService,
        UserSearchTagInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        var result = service.Add(input);
        if (result.Status != ResponseStatus.Success)
            return result;
        tagService.AddViewToTag(input.Tag,currentUser);
        return result;
    }

    [GraphQLName("userSearchTag_removeUserSearchTag")]
    public ResponseStatus Remove(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] UserSearchTagService service,
            string tag)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.DeleteSearchedTag(currentUser.Id, tag);
    }

}