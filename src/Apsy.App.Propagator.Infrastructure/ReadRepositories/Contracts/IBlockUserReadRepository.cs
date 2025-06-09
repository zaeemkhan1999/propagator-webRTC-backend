namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IBlockUserReadRepository : IRepository<BlockUser>
{

    bool IsBlocked(int userId, int blockerId);

    void SetMutual(int userId, int blockerId);
    BlockUser UnblockUser(int blockerId, int blockedId);
}
