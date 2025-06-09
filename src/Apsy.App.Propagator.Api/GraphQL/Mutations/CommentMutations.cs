




namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CommentMutations
{
    [GraphQLName("comment_createComment")]
    public async Task<ResponseBase<Comment>> CreateComment(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
             CommentInput commentInput,
             [Service] IPostService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        commentInput.UserId = currentUser.Id;
        return await service.CreateComment(commentInput);
    }

    [GraphQLName("comment_removeComment")]
    public async Task<ResponseStatus> Remove(
                      [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                      [Service] ICommentService service,
                      int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.DeleteComment(entityId,authentication.CurrentUser);
    }

    [GraphQLName("comment_undoRemoveComment")]
    public async Task<ResponseStatus> undoRemove(
                  [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                  [Service] ICommentService service,
                  int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        if (authentication.CurrentUser.UserTypes != UserTypes.Admin && authentication.CurrentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.UndoDeleteComment(entityId, authentication.CurrentUser);
    }

    [GraphQLName("comment_removeListComment")]
    public async Task<ResponseBase<bool>> RemoveList(
                      [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                      [Service] ICommentService service,
                      List<int> ids)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.SoftDeleteAll(ids, authentication.CurrentUser);
    }

    [GraphQLName("comment_updateComment")]
    public ResponseBase<Comment> Update(
                         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] ICommentService service,
                        CommentInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Update(input);
    }
}