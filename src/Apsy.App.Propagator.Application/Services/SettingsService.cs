using Propagator.Common.Services.Contracts;
using User = Apsy.App.Propagator.Domain.Entities.User;


namespace Apsy.App.Propagator.Application.Services;
public class SettingsService : ServiceBase<Settings, SettingsInput>, ISettingsService
{
    public SettingsService(
       ISettingsRepository repository,
       IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly ISettingsRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

   
}