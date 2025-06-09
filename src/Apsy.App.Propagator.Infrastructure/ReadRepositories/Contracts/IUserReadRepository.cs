using Aps.CommonBack.Auth.Repositories.Contracts;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserReadRepository : IUserRepositoryBase<User>
{
    IQueryable<User> GetAllUser();
    User GetUserByAppUserId(string appUserId);
    Task<bool> IsValidUserAsync(AppUser users);
    Task<User> DeleteUserAndDependencies(int userId);
    UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user);
    UserRefreshTokens GetSavedRefreshTokens(string username, string refreshToken);
    void DeleteUserRefreshTokens(string username, string refreshToken);
    int SaveCommit();

     User GetUserId(int Id);
    IQueryable<User> GetUser();
    IQueryable<SecurityAnswer> GetSecurityAnswer();
    IQueryable<UserLogin> GetUserLogin();
    IQueryable<Post> GetPost();
    IQueryable<Article> GetArticles();
    IQueryable<UserFollower> GetUserFollowers();
    IQueryable<AdminTodayLimitation> GetAdminTodayLimitation();
    IQueryable<Support> GetSupport();

    Task<bool> isUserAvailable(int userId);
    IQueryable<User> GetUser(int userId);
    IQueryable<User> GetUsers();
    IQueryable<User> GetUsers(PublicNotification notif);
    List<int> GetUserIdsFromUserNames(int userId, string[] userNames);
    User GetUserByIdWithAppUser(int userId);

    IQueryable<UsersSubscription> GetUsersSubscription();
    IQueryable<ResetPasswordRequest> GetResetPasswordRequest();
    User GetUserByIdAsync(int userId);
}
