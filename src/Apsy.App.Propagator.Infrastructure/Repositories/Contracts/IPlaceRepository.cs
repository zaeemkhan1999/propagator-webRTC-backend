namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IPlaceRepository
 : IRepository<Place>
{

#region functions
    bool isPlaceExists(string location);
#endregion
}
