namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class BlockUserReadRepository : Repository<BlockUser, DataWriteContext>, IBlockUserReadRepository
{
    public BlockUserReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;


    public bool IsBlocked(int userId, int blockerId)
    {
        return GetDbSet().Any(a => a.BlockerId == blockerId && a.BlockedId == userId);
    }

    public void SetMutual(int userId, int blockerId)
    {
        var userFollower = Get(a => a.BlockerId == blockerId && a.BlockedId == userId);
        if (userFollower != null)
        {
            userFollower.IsMutual = true;
        }
    }
    public BlockUser UnblockUser(int blockerId,int blockedId)
    {
        return context.BlockUser.Where(a=> a.BlockerId == blockerId && a.BlockedId == blockedId).FirstOrDefault();
    }
}
