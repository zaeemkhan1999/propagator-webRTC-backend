namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserVisitLinkMutations
{
    [GraphQLName("userVisitLink_createUserVisitLink")]
    public ResponseBase<UserVisitLink> Create(
              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
              [Service(ServiceKind.Default)] IUserVisitLinkService service,
              UserVisitLinkInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("userVisitLink_removeUserVisitLink")]
    public ResponseStatus Remove(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IUserVisitLinkService service,
                        string text,
                        string link,
                        int? postId,
                        int? articleId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.DeleteUserVisitLink(currentUser.Id, text, link, postId, articleId);
    }

    [GraphQLName("userVisitLink_removeAllUserVisitLink")]
    public async Task<ResponseBase<bool>> RemoveAll(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IUserVisitLinkService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.DeleteAllUserVisitLink();
    }

}
