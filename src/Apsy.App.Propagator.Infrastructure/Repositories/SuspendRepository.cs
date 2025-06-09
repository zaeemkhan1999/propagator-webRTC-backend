namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SuspendRepository : Repository<Suspend, DataReadContext>, ISuspendRepository
{
    public SuspendRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataReadContext context;

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