using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class FollowQueries
{

    [GraphQLName("follow_getFollowers")]
    public ListResponseBase<UserFollower> GetFollowers([Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service] IFollowerReadService service, int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetFollowers(userId);
    }

    [GraphQLName("follow_getFollowings")]
    public ListResponseBase<UserFollower> GetFollowings([Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service] IFollowerReadService service, int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetFollowings(userId);
    }

    [GraphQLName("follow_getFollowings_old")]
    public ListResponseBase<UserFollower> Get(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service] IFollowerReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }

    [GraphQLName("follow_isMyFollower")]
    public ResponseBase<bool> IsMyFollower(
                       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                       [Service] IFollowerReadService followService,
                       int otherUserId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return new(followService.Any(c => c.FollowedId == currentUser.Id && c.FollowerId == otherUserId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted));
    }

    [GraphQLName("follow_isMyFollowing")]
    public ResponseBase<bool> IsMyFollowing(
                                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                       [Service] IFollowerReadService followService,
                       int otherUserId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return new(followService.Any(c => c.FollowerId == currentUser.Id && c.FollowedId == otherUserId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted));
    }

    [GraphQLName("follow_getFollowInfo")]
    public ResponseBase<FollowInfoDto> GetFollowInfo(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service] IFollowerReadService followService,
                    int otherUserId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return followService.GetUserFollowInfo(otherUserId, authentication.CurrentUser);
    }

    [GraphQLName("follow_getMyFollowersFollowers")]
    public ListResponseBase<UserFollower> GetMyFollowersFollowers(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service] IFollowerReadService followService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return followService.GetMyFollowersFollowers(authentication.CurrentUser);
    }

}