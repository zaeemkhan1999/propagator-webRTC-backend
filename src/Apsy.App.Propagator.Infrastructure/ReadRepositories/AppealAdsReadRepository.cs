namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class AppealAdsReadRepository
 : Repository<AppealAds,DataWriteContext>, IAppealAdsReadRepository
{
public AppealAdsReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	public DataWriteContext context;

    #endregion
    #region functions
    public Task<AppealAds> GetbyId(int Id)
    {
        return context.AppealAds.Where(d => d.Id == Id).Include(d => d.Ads).FirstOrDefaultAsync();
        
    }
    #endregion
}
