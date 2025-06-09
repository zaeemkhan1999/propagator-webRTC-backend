namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IUserSearchPostRepository
 : IRepository<UserSearchPost>
{

    #region functions
    IQueryable<UserSearchPost> GetUserSearchPost();
    IQueryable<User> GetUser();
#endregion
}
