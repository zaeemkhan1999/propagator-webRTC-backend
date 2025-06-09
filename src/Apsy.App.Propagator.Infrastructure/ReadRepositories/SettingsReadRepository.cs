namespace Apsy.App.Propagator.Infrastructure.Repositories;
public class SettingsReadRepository : Repository<Settings, DataWriteContext>, ISettingsReadRepository
{
    public SettingsReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataWriteContext context;
    public Settings GetFirstSettings()
    {
        var settings = context.Settings.AsNoTracking().FirstOrDefault();
        return settings;
    }
    public IQueryable<Settings> GetAllSettings()
    {
        var settings = context.Settings.AsNoTracking().AsQueryable();
        return settings;
    }

}