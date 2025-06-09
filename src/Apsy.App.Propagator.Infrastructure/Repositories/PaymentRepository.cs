namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class PaymentRepository
 : Repository<Payment, DataReadContext>, IPaymentRepository
{
    public PaymentRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataReadContext context;

	#endregion
	#region functions

	public Payment GetPayment(int id, bool checkDeleted)
    {
        var payment = context.Payment.Where(x => x.Id == id).FirstOrDefault();

        return payment;
    }

	public async Task<Payment> GetPayment(int id)
	{
		var payment = await context.Payment.Where(x => x.Id == id).FirstOrDefaultAsync();

		return payment;
	}

	//public IQueryable<Post> GetPost(int id)
	//{
	//	var post = context.Post.Where(p => p.Id == id).AsNoTracking().AsQueryable();
	//	return post;
	//}

	//public IQueryable<Ads> GetAds(int id)
	//{
	//	var ads = context.Ads.Where(p => p.Id == id).AsNoTracking().AsQueryable();
	//	return ads;
	//}

	//public IQueryable<Article> GetArticle(int id)
	//{
	//	var article = context.Article.Where(p => p.Id == id).AsNoTracking().AsQueryable();
	//	return article;
	//}

	//public IQueryable<Settings> GetSettings()
	//{
	//	var settings = context.Setting.AsNoTracking().AsQueryable();
	//	return settings;
	//}


	public async Task<TCustomEntity> AddAsync<TCustomEntity>(TCustomEntity entity, bool saveNow = true) where TCustomEntity : EntityDef
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await Context.Set<TCustomEntity>().AddAsync(entity);

        if (saveNow)
        {
            await SaveChangesAsync(Context);
        }

        return entity;
    }

    public virtual async Task UpdateRangeAsync<TCustomEntity>(IEnumerable<TCustomEntity> entities, bool saveNow = true) where TCustomEntity : EntityDef
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        Context.Set<TCustomEntity>().UpdateRange(entities);

        if (saveNow)
        {
            await Context.SaveChangesAsync();
        }
    }

    #endregion
}
