using Microsoft.AspNetCore.Identity;


namespace Apsy.App.Propagator.Infrastructure.Repositories;
public class UserReadRepository : UserRepositoryBase<DataWriteContext, User>, IUserReadRepository
{
    public UserReadRepository(
        UserManager<AppUser> userManager,
        IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        _context = dbContextFactory.CreateDbContext();
        _userManager = userManager;
    }

    private readonly DataWriteContext _context;
    private readonly UserManager<AppUser> _userManager;

    public IQueryable<User> GetAllUser()
    {
        var query = _context.User.AsQueryable();
        return query;
    }
    public User GetUserByAppUserId(string appUserId)
    {
        return _context.Set<AppUser>().Where(c => c.Id == appUserId).Select(c => c.User).FirstOrDefault();
    }
    public User GetUserId(int Id)
    {
        return _context.User.Where(x=>x.Id==Id).FirstOrDefault();
    }

    public UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user)
    {
        _context.Set<UserRefreshTokens>().Add(user);
        return user;
    }

    public void DeleteUserRefreshTokens(string username, string refreshToken)
    {
        var item = _context.Set<UserRefreshTokens>().FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
        if (item != null)
        {
            _context.Set<UserRefreshTokens>().Remove(item);
        }
    }

    public UserRefreshTokens GetSavedRefreshTokens(string username, string refreshToken)
    {
        return _context.Set<UserRefreshTokens>().FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive);
    }

    public int SaveCommit()
    {
        return _context.SaveChanges();
    }

    public async Task<bool> IsValidUserAsync(AppUser users)
    {
        var user = _userManager.Users.FirstOrDefault(o => o.UserName == users.UserName);
        var result = await _userManager.CheckPasswordAsync(user, users.PasswordHash);
        return result;
    }

    public async Task<User> DeleteUserAndDependencies(int userId)
    {
        var user = Context.Set<User>().FirstOrDefault(x => x.Id == userId);
        if (user == null) return null;

        // user.IsDeleted = true;
        var deleteAccountIdentifier = "deleted Account" + " | " + Guid.NewGuid().ToString();
        user.IsDeletedAccount = true;
        user.DeleteAccountDate = DateTime.UtcNow;
        user.Email = deleteAccountIdentifier;
        user.ExternalId = deleteAccountIdentifier;
        user.Username = deleteAccountIdentifier;
        user.IsActive = false;
        user.DisplayName = deleteAccountIdentifier;
        user.Bio = deleteAccountIdentifier;
        user.ImageAddress = deleteAccountIdentifier;
        user.Cover = deleteAccountIdentifier;
        user.Location = deleteAccountIdentifier;
        //Context.Set<User>().Update(user);

        var appUser = await Context.Users.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
        appUser.Email = deleteAccountIdentifier;
        appUser.NormalizedEmail = deleteAccountIdentifier;
        appUser.UserName = deleteAccountIdentifier;
        appUser.NormalizedUserName = deleteAccountIdentifier;
        appUser.PhoneNumberConfirmed = false;
        appUser.PhoneNumber = deleteAccountIdentifier;
        appUser.EmailConfirmed = false;
        //Context.Update(appUser);
        await Context.SaveChangesAsync();

        return user;
    }
    public IQueryable<User>GetUser()
    {
        var query = Context.User.AsQueryable();
        return query;
    }
    public IQueryable<UsersSubscription>GetUsersSubscription()
    {
        var query = Context.UsersSubscription.AsQueryable();
        return query;
    }
    public IQueryable<ResetPasswordRequest> GetResetPasswordRequest()
    {
        var query = Context.ResetPasswordRequest.AsQueryable();
        return query;
    }
    public User GetUserByIdAsync(int userId)
    {
        var query =Context.User.Find(userId);
        return query;
    }
    public IQueryable<SecurityAnswer> GetSecurityAnswer()
    {
        var query = Context.SecurityAnswer.AsQueryable();
        return query;
    }
    public IQueryable<UserLogin> GetUserLogin()
    {
        var query = Context.UserLogin.AsQueryable();
        return query;
    }
    public IQueryable<Post> GetPost()
    {
        var query = Context.Post.AsQueryable();
        return query;
    }
    public IQueryable<Article>GetArticles()
    {
        var query = Context.Article.AsQueryable();
        return query;
    }
    public IQueryable<UserFollower>GetUserFollowers()
    {
        var query = Context.UserFollower.AsQueryable();
        return query;
    }
    public IQueryable<AdminTodayLimitation> GetAdminTodayLimitation()
    {
        var query = Context.AdminTodayLimitation.AsQueryable();
        return query;
    }
    public IQueryable<Support> GetSupport()
    {
        var query = Context.Support.AsQueryable();
        return query;
    }

    public Task<bool> isUserAvailable(int userId)
    {
        return Context.User.AnyAsync((User a) => a.Id == userId);
    }

    public IQueryable<User> GetUser(int userId)
    {
        return Context.User.Where(c => c.Id == userId).AsNoTracking().AsQueryable();
    }

    public IQueryable<User> GetUsers()
    {
        var user = _context.User
                .AsNoTracking().AsQueryable();
        return user;
    }

    public IQueryable<User> GetUsers(PublicNotification notif)
    {
        var user = _context.User.Where(d => d.Gender == notif.Gender && (DateTime.Today.Year - d.DateOfBirth.Year) >= notif.FromAge && (DateTime.Today.Year - d.DateOfBirth.Year) <= notif.ToAge)
                .AsNoTracking().AsQueryable();
        return user;
    }

    public List<int> GetUserIdsFromUserNames(int userId, string[] userNames)
    {
        return Context.User.
            Where(x => x.Id != userId && userNames.Contains(x.Username))
            .Select(x => x.Id).ToList();
    }

    public User GetUserByIdWithAppUser(int userId)
    {
        var user = _context.User.Include(x => x.AppUser).FirstOrDefault(c => c.Id == userId);
        return user;

    }
}