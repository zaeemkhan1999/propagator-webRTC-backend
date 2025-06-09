namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IPlaceReadRepository
 : IRepository<Place>
{

#region functions
    bool isPlaceExists(string location);
#endregion
}
