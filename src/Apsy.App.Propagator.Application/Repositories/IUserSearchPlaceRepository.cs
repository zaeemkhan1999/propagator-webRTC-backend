namespace Apsy.App.Propagator.Application.Repositories;

public interface  IUserSearchPlaceRepository
 : IRepository<UserSearchPlace>
{

    #region functions
    IQueryable<UserSearchPlace> GetUserSearchPlace();
    IQueryable<User> GetUser();
#endregion
}
