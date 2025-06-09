namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IAppealAdsReadRepository : IRepository<AppealAds>
{
    public Task<AppealAds> GetbyId(int Id);
}
