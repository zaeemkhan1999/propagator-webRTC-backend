namespace Apsy.App.Propagator.Application.Services;

public class BlockUserService : ServiceBase<BlockUser, BlockUserInput>, IBlockUserService
{
    public BlockUserService(IBlockUserRepository repository) : base(repository)
    {
        this.repository = repository;
    }

    private readonly IBlockUserRepository repository;

    public ResponseBase<BlockUser> AddBlocked(BlockUserInput input)
    {
        int blockerIdValueOrDefault = input.BlockerId.GetValueOrDefault();
        int blockedIdValueOrDefault = input.BlockedId.GetValueOrDefault();
        if (blockerIdValueOrDefault <= 0 || blockedIdValueOrDefault <= 0)
        {
            return ResponseBase<BlockUser>.Failure(ResponseStatus.NotEnoghData);
        }

        if (blockedIdValueOrDefault == blockerIdValueOrDefault)
        {
            return ResponseBase<BlockUser>.Failure(ResponseStatus.NotAllowd);
        }

        BlockUser userBlock = input.Adapt<BlockUser>();
        if (repository.IsBlocked(blockedIdValueOrDefault, blockerIdValueOrDefault))
        {
            return ResponseBase<BlockUser>.Failure(ResponseStatus.AlreadyExists /*AlreadyFollowed*/);
        }

        if (repository.IsBlocked(blockerIdValueOrDefault, blockedIdValueOrDefault))
        {
            userBlock.IsMutual = true;
        }

        repository.SetMutual(blockerIdValueOrDefault, blockedIdValueOrDefault);
        repository.Add(userBlock);

        var followerAndFollwings = repository.Where<UserFollower>(c =>
                    (c.FollowerId == blockerIdValueOrDefault && c.FollowedId == blockedIdValueOrDefault)
                    ||
                    (c.FollowedId == blockerIdValueOrDefault && c.FollowerId == blockedIdValueOrDefault));
        repository.RemoveRange(followerAndFollwings);


        return ResponseBase<BlockUser>.Success(userBlock);
    }

    public ResponseBase UnBlock(BlockUserInput input)
    {
        int blockerId = input.BlockerId.GetValueOrDefault();
        int blockedId = input.BlockedId.GetValueOrDefault();
        if (blockerId <= 0 || blockedId <= 0)
        {
            return ResponseBase.Failure(ResponseStatus.NotEnoghData);
        }

        if (blockedId == blockerId)
        {
            return ResponseBase.Failure(ResponseStatus.AlreadyExists);
        }

        BlockUser val = repository.UnblockUser(blockerId,blockedId);
        if (val == null)
        {
            return ResponseBase.Failure(ResponseStatus.NotFound);
        }

        if (val.IsMutual)
        {
            BlockUser val2 = repository.UnblockUser(blockedId, blockerId);
            val2.IsMutual = false;
            ((IRepository<BlockUser>)repository).Update<BlockUser>(val2);
        }

        ((IRepository<BlockUser>)repository).Delete<BlockUser>(val);
        return ResponseBase.Success();
    }


}
