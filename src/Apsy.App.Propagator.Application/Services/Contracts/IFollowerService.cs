namespace Apsy.App.Propagator.Application.Services.Contracts
{
    public interface IFollowerService : IServiceBase<UserFollower, FollowerInput>
    {
        Task<ResponseBase<UserFollower>> AddFollowed(FollowerInput input, User currentUser);
        Task<ResponseBase> Unfollow(FollowerInput input, User currentUser);
        ResponseBase<UserFollower> ConfirmFollowRequest(FollowerInput input);
        ResponseBase<UserFollower> RejectFollowRequest(FollowerInput input);
    }
}