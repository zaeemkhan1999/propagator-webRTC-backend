namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserSearchPostReadRepository
 : IRepository<UserSearchPost>
{

    #region functions
    IQueryable<UserSearchPost> GetUserSearchPost();
    IQueryable<User> GetUser();
#endregion
}
