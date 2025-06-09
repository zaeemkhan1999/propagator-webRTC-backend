namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISettingsRepository : IRepository<Settings>
{
    Settings GetFirstSettings(); 
    IQueryable<Settings> GetAllSettings(); 
}