namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISettingsReadRepository : IRepository<Settings>
{
    Settings GetFirstSettings(); 
    IQueryable<Settings> GetAllSettings(); 
}