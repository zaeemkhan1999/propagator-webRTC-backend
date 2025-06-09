namespace Apsy.App.Propagator.Application.Repositories;

public interface  IUserSearchTagRepository
 : IRepository<UserSearchTag>
{

    #region functions
    IQueryable<UserSearchTag> GetUserSearchTag();
    IQueryable<User> GetUser();
#endregion
}
