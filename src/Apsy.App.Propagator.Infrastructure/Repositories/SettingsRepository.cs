namespace Apsy.App.Propagator.Infrastructure.Repositories;
public class SettingsRepository : Repository<Settings, DataReadContext>, ISettingsRepository
{
    public SettingsRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataReadContext context;
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