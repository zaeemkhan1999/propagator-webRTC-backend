namespace Apsy.App.Propagator.Application.Services;

public class PlaceService
 : ServiceBase<Place, PlaceInput>, IPlaceService
{
    private IPlaceRepository repository;

    public PlaceService(IPlaceRepository repository) : base(repository)
    {
        this.repository = repository;

    }
}
