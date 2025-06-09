namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class BlockUserRepository : Repository<BlockUser, DataReadContext>, IBlockUserRepository
{
    public BlockUserRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;


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
