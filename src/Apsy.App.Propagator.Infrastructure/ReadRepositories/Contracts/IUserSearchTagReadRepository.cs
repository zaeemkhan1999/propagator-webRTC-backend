namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserSearchTagReadRepository
 : IRepository<UserSearchTag>
{

    #region functions
    IQueryable<UserSearchTag> GetUserSearchTag();
    IQueryable<User> GetUser();
#endregion
}
