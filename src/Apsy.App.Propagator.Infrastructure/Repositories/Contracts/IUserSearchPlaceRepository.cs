namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IUserSearchPlaceRepository
 : IRepository<UserSearchPlace>
{

    #region functions
    IQueryable<UserSearchPlace> GetUserSearchPlace();
    IQueryable<User> GetUser();
#endregion
}
