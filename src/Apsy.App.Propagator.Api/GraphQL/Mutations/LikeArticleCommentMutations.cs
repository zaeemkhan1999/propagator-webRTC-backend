namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class LikeArticleCommentMutations
{
    [GraphQLName("likeArticleComment_LikeArticleComment")]
    public async Task<ResponseBase<LikeArticleComment>> LikeArticleComment(
                               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                               [Service(ServiceKind.Default)] ILikeArticleCommentService service,
                               [Service(ServiceKind.Default)] IArticleCommentService articleCommentService,
                               [Service(ServiceKind.Default)] IUserService userService,
                               [Service(ServiceKind.Default)] IPublisher publisher,
                               LikeArticleCommentInput input)
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

        try
        {
            var recieverId = articleCommentService.Get(result.Result.ArticleCommentId)?.Result?.UserId;
            var reciever = userService.Get((int)recieverId);
            if (reciever.Status == ResponseStatus.Success && reciever.Result?.LikeNotification == true && currentUser.Id != (int)recieverId)
                await publisher.Publish(new LikeArticleCommentEvent(result.Result.Id, currentUser.Id, (int)recieverId));
        }
        catch
        {
        }
        return result;
    }

    [GraphQLName("likeArticleComment_unLikeArticleComment")]
    public ResponseStatus UnLikeArticleComment(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] ILikeArticleCommentService service,
                            LikeArticleCommentInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        input.UserId = currentUser.Id;
        return service.UnLikeArticleComment(input);
    }
}