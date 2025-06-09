namespace Apsy.App.Propagator.Application.Repositories;

public interface  IPlaceRepository
 : IRepository<Place>
{

#region functions
    bool isPlaceExists(string location);
#endregion
}
