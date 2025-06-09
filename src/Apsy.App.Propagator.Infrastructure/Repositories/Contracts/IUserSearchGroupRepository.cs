namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IUserSearchGroupRepository
 : IRepository<UserSearchGroup>
{

    #region functions
    IQueryable<UserSearchGroup> GetUserSearchGroup();
    IQueryable<User> GetUser();
    IQueryable<User> GetUserById(int id);
    Conversation FindConversation(int id);
#endregion
}
