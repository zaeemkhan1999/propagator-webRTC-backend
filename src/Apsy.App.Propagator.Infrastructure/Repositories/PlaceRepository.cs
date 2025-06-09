

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class PlaceRepository
 : Repository<Place,DataReadContext>,IPlaceRepository
{
public PlaceRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;


    #endregion
    #region functions

    public bool isPlaceExists(string location)
    {
       return context.Place.Any(c => c.Location == location);
    }
    #endregion
}
