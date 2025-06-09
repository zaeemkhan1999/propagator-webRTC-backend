

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class PlaceReadRepository
 : Repository<Place,DataWriteContext>,IPlaceReadRepository
{
public PlaceReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;


    #endregion
    #region functions

    public bool isPlaceExists(string location)
    {
       return context.Place.Any(c => c.Location == location);
    }
    #endregion
}
