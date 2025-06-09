namespace Apsy.App.Propagator.Application.Repositories;

public interface IBlockUserRepository : IRepository<BlockUser>
{

    bool IsBlocked(int userId, int blockerId);

    void SetMutual(int userId, int blockerId);
    BlockUser UnblockUser(int blockerId, int blockedId);
}
