namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IAppealAdsRepository : IRepository<AppealAds>
{
    public Task<AppealAds> GetbyId(int Id);
}
