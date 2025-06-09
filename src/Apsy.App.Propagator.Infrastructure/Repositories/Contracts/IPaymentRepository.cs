

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IPaymentRepository
 : IRepository<Payment>
{

	#region functions
	#endregion

	Payment GetPayment(int id, bool checkDeleted);
	Task<Payment> GetPayment(int id);
	//IQueryable<Post> GetPost(int id);
	//IQueryable<Ads> GetAds(int id);
	//IQueryable<Article> GetArticle(int id);
	//IQueryable<Settings> GetSettings();
	Task<TCustomEntity> AddAsync<TCustomEntity>(TCustomEntity entity, bool saveNow = true) where TCustomEntity : EntityDef;
    Task UpdateRangeAsync<TCustomEntity>(IEnumerable<TCustomEntity> entities, bool saveNow = true) where TCustomEntity : EntityDef;
}
