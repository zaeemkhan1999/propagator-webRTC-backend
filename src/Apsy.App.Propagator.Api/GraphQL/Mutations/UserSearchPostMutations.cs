namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserSearchPostMutations
{
    [GraphQLName("userSearchPost_createUserSearchPost")]
    public ResponseBase<UserSearchPost> Create(
              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
              [Service(ServiceKind.Default)] IUserSearchPostService service,
              UserSearchPostInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("userSearchPost_removeUserSearchPost")]
    public ResponseStatus Remove(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IUserSearchPostService service,
            int postId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return service.DeleteSearchedPost(currentUser.Id, postId);
    }

}
