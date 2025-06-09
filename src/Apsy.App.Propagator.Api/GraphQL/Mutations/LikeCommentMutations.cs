namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class LikeCommentMutations
{
    [GraphQLName("likeComment_likeComment")]
    public async Task<ResponseBase<LikeComment>> LikeComment(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] ILikeCommentService service,
                                [Service(ServiceKind.Default)] ICommentService commentService,
                                [Service(ServiceKind.Default)] IUserService userService,
                                [Service(ServiceKind.Default)] IPublisher publisher,
                                LikeCommentInput input)
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
            var recieverId = commentService.Get(result.Result.CommentId)?.Result?.UserId;
            var reciever = userService.Get((int)recieverId);
            if (reciever.Status == ResponseStatus.Success && reciever.Result?.LikeNotification == true && currentUser.Id != (int)recieverId)
                await publisher.Publish(new LikeCommentEvent(result.Result.Id, currentUser.Id, (int)recieverId));
        }
        catch
        {
        }
        return result;
    }

    [GraphQLName("likeComment_unLikeComment")]
    public ResponseStatus UnLikeComment(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] ILikeCommentService service,
                                LikeCommentInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.UnlikeComment(input);
    }
}