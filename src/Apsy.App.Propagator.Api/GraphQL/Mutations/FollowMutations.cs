namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class FollowMutations
{
    [GraphQLName("follow_followUser")]
    public async Task<ResponseBase<UserFollower>> FollowUser(
                               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service] IFollowerService service,
                                FollowerInput followerInput)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        followerInput.FollowedAt = DateTime.UtcNow;
        followerInput.FollowerId = currentUser.Id;
        followerInput.FolloweAcceptStatus = FolloweAcceptStatus.Pending;
        return await service.AddFollowed(followerInput, authentication.CurrentUser);
    }

    [GraphQLName("follow_unFollowUser")]
    public async ValueTask<ResponseBase<ResponseStatus>> UnfollowUser(
                                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                            [Service] IFollowerService followService,
                                            FollowerInput followerInput)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return ResponseBase<ResponseStatus>.Failure(authentication.Status);
        }

        User currentUser = authentication.CurrentUser;

        followerInput.FollowerId = currentUser.Id;
        var result = await followService.Unfollow(followerInput, authentication.CurrentUser);
        if (result.Status != ResponseStatus.Success)
            return ResponseBase<ResponseStatus>.Failure(result);
        return ResponseBase<ResponseStatus>.Success(result.Status);
    }

    [GraphQLName("follow_confirmFollowRequest")]
    public ResponseBase<UserFollower> ConfirmFollowRequest(
                                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                            [Service] IFollowerService service,
                                            FollowerInput followerInput)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        followerInput.FollowedId = currentUser.Id;
        return service.ConfirmFollowRequest(followerInput);
    }

    [GraphQLName("follow_rejectFollowRequest")]
    public ResponseBase<UserFollower> RejectFollowRequest(
                                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                            [Service] IFollowerService service,
                                            FollowerInput followerInput)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        followerInput.FollowedId = currentUser.Id;
        return service.RejectFollowRequest(followerInput);
    }
}