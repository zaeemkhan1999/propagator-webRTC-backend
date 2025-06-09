namespace Apsy.App.Propagator.Application.Repositories;

public interface IAppealAdsRepository : IRepository<AppealAds>
{
    public Task<AppealAds> GetbyId(int Id);
}
