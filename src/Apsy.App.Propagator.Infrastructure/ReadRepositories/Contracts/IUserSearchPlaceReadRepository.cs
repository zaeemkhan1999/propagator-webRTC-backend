namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IUserSearchPlaceReadRepository
 : IRepository<UserSearchPlace>
{

    #region functions
    IQueryable<UserSearchPlace> GetUserSearchPlace();
    IQueryable<User> GetUser();
#endregion
}
