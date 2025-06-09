


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IAdReadRepository : IRepository<Ads>
{
    public IQueryable<AdsDto> GetbyId(int Id,int UserId);
    public Ads GetbyAdsId(int Id);
    public IQueryable<AdsDto> GetbyAll(int UserId);
    public IQueryable<AdsDto> GetAdsesForSlider(List<int> ignoredAdsIds, User currentuser);

	IQueryable<Ads> GetAdById(int id);
	IQueryable<Ads> GetAdsByPostId(int postId);
}
