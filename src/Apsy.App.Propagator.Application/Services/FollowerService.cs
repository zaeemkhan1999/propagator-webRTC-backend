    namespace Apsy.App.Propagator.Application.Services;

public class FollowerService : ServiceBase<UserFollower, FollowerInput>, IFollowerService

{
    public FollowerService(
        IFollowerRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IBlockUserRepository blockUserRepository,
        IPublisher publisher) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _blockUserRepository = blockUserRepository;
        _publisher = publisher;
    }
    private readonly IFollowerRepository repository;
    private readonly IBlockUserRepository _blockUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    public async Task<ResponseBase<UserFollower>> AddFollowed(FollowerInput input, User currentUser)
    {

        if (_blockUserRepository.IsBlocked((int)input.FollowedId ,currentUser.Id))
            return CustomResponseStatus.UserAlreadyBlockedByContentOwner;

        int followerId = input.FollowerId.GetValueOrDefault();
        int followedId = input.FollowedId.GetValueOrDefault();
        if (followerId <= 0 || followedId <= 0)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.NotEnoghData);
        }

        if (followedId == followerId)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.SelfFollowingNotAllowed);
        }

        var followedUser = repository.GetFollowerUser(followedId);
        if (followedUser is null)
            return ResponseStatus.NotFound;

        if (repository.GetUserFollowerRejected(followerId,followedId) is not null)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.AlreadyFollowed);
        }
        UserFollower followe = input.Adapt<UserFollower>();
        if (!followedUser.PrivateAccount)
        {
            followe.FolloweAcceptStatus = FolloweAcceptStatus.Accepted;
            if (repository.IsFollowed(followerId, followedId))
                followe.IsMutual = true;
            repository.SetMutual(followerId, followedId);
        }
        var isBlocked = _blockUserRepository.IsBlocked(followedId, (int)input.FollowerId);
        if (isBlocked)
            return CustomMessagingResponseStatus.CanNotFollowBlocker;
        repository.Add(followe);
        try
        {
            var reciver = repository.GetFollowerUser(followe.FollowedId);
            await _publisher.Publish(new FollowRequestEvent(followe.Id, currentUser.Id, followe.FollowedId));
        }
        catch
        {
        }

        return ResponseBase<UserFollower>.Success(followe);
    }

    public async Task<ResponseBase> Unfollow(FollowerInput input, User currentUser)
    {

        int followerId = input.FollowerId.GetValueOrDefault();
        int followedId = input.FollowedId.GetValueOrDefault();
        if (followerId <= 0 || followedId <= 0)
        {
            return ResponseBase.Failure(ResponseStatus.NotEnoghData);
        }

        if (followedId == followerId)
        {
            return ResponseBase.Failure(ResponseStatus.SelfFollowingNotAllowed);
        }
        UserFollower userFollow = repository.GetUserFollower(followerId, followedId);
        if (userFollow == null)
        {
            return ResponseBase.Failure(ResponseStatus.NotFound);
        }
        var isBlocked = _blockUserRepository.IsBlocked(followedId,(int)input.FollowerId);
        if (isBlocked)
            return CustomMessagingResponseStatus.CanNotFollowBlocker;

        if (userFollow.IsMutual)
        {
            UserFollower val2 = repository.GetUserFollower(followedId, followerId);
            val2.IsMutual = false;
            repository.Update(val2);
        }

        repository.Remove(userFollow);


        try
        {
            await _publisher.Publish(new UnFollowRequestEvent(userFollow.Id, currentUser.Id, userFollow.FollowedId));
        }
        catch
        {
        }

        return ResponseBase.Success();
    }

    public ResponseBase<UserFollower> ConfirmFollowRequest(FollowerInput input)
    {
        int followerId = input.FollowerId.GetValueOrDefault();
        int followedId = input.FollowedId.GetValueOrDefault();
        if (followerId <= 0 || followedId <= 0)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.NotEnoghData);
        }

        if (followedId == followerId)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.SelfFollowingNotAllowed);
        }

        if (repository.Any((a) => a.FolloweAcceptStatus == FolloweAcceptStatus.Rejected))
            return CustomResponseStatus.AlreadyExistsReject;

        UserFollower userFollower = repository.GetUserFollowerRejected(followerId, followedId);
        if (userFollower == null)
        {
            return ResponseStatus.NotFound;
        }

        var exist = repository.GetPendingFollwoRequest(followerId, followedId);


        if (!exist)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.NotFound);
        }

        if (repository.GetById<User>(followedId) == null)
        {
            return ResponseBase<UserFollower>.Failure(ResponseStatus.NotFound);
        }

        userFollower.FolloweAcceptStatus = FolloweAcceptStatus.Accepted;
        repository.SetMutual(followerId, followedId);
        repository.Update(userFollower);
        return ResponseBase<UserFollower>.Success(userFollower);
    }

    public ResponseBase<UserFollower> RejectFollowRequest(FollowerInput input)
    {
        int followerId = input.FollowerId.GetValueOrDefault();
        int followedId = input.FollowedId.GetValueOrDefault();
        if (followerId <= 0 || followedId <= 0)
        {
            return ResponseStatus.NotEnoghData;
        }

        if (followedId == followerId)
        {
            return ResponseStatus.SelfFollowingNotAllowed;
        }

        if (repository.Any((a) => a.FolloweAcceptStatus == FolloweAcceptStatus.Rejected))
            return CustomResponseStatus.AlreadyExistsReject;

        UserFollower userFollow = repository.GetUserFollowerRejected(followerId,followedId);
        if (userFollow == null)
        {
            return ResponseStatus.NotFound;
        }
        if (userFollow.IsMutual)
        {
            UserFollower val2 = repository.GetUserFollowerRejected(followedId,followerId);
            val2.IsMutual = false;
            repository.Update(val2);
        }
        userFollow.FolloweAcceptStatus = FolloweAcceptStatus.Rejected;
        userFollow.IsDeleted = true;


        repository.Remove(userFollow);
        return ResponseBase<UserFollower>.Success(userFollow);
    }
}