namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class AppealAdsRepository
 : Repository<AppealAds,DataReadContext>, IAppealAdsRepository
{
public AppealAdsRepository(IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	public DataReadContext context;

    #endregion
    #region functions
    public Task<AppealAds> GetbyId(int Id)
    {
        return context.AppealAds.Where(d => d.Id == Id).Include(d => d.Ads).FirstOrDefaultAsync();
        
    }
    #endregion
}
