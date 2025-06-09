namespace Apsy.App.Propagator.Application.Repositories;

public interface ISettingsRepository : IRepository<Settings>
{
    Settings GetFirstSettings(); 
    IQueryable<Settings> GetAllSettings(); 
}