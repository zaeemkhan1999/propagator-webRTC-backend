namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SuspendReadRepository : Repository<Suspend, DataWriteContext>, ISuspendReadRepository
{
    public SuspendReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataWriteContext context;

    public User GetUserById(int id)
    {
        var query = context.User.Where(x => x.Id == id).FirstOrDefault();

        return query;
    }
    public IQueryable<AdminTodayLimitation> GetAdminTodayLimitation()
    {
        var query = context.AdminTodayLimitation.AsQueryable();
        return query;
    }
}