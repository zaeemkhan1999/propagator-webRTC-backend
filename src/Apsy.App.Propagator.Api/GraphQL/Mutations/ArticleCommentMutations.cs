namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class ArticleCommentMutations
{
    [GraphQLName("articleComment_createArticleComment")]
    public async Task<ResponseBase<ArticleComment>> Create(
                       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                       [Service(ServiceKind.Default)] IArticleCommentService service,
                        ArticleCommentInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return await service.AddArticleComment(input,currentUser);
    }

    [GraphQLName("articleComment_removeArticleComment")]
    public async Task<ResponseStatus> RemoveArticleComment(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service(ServiceKind.Default)] IArticleCommentService service,
                int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.SoftDeleteArticleComment(entityId, authentication.CurrentUser);
    }

    [GraphQLName("comment_undoArticleComment")]
    public async Task<ResponseStatus> undoRemove(
              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
              [Service] IArticleCommentService service,
              int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        if (authentication.CurrentUser.UserTypes != UserTypes.Admin && authentication.CurrentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.UndoDeleteArticleComment(entityId, authentication.CurrentUser);
    }

    [GraphQLName("articleComment_removeListArticleComment")]
    public async Task<ResponseBase<bool>> RemoveList(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IArticleCommentService service,
        List<int> ids)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.IsDeletedAccount) return CustomResponseStatus.UserIsNotActive;

        return await service.SoftDeleteAll(ids, authentication.CurrentUser);
    }

    [GraphQLName("articleComment_updateArticleComment")]
    public ResponseBase<ArticleComment> Update(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticleCommentService service,
                        ArticleCommentInput input)
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