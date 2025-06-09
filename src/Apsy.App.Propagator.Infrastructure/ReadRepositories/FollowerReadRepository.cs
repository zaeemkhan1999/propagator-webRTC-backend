namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class FollowerReadRepository : Repository<UserFollower, DataWriteContext>, IFollowerReadRepository
{
    public FollowerReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;

    public bool IsFollowed(int userId, int followerId)
    {
        return GetDbSet().Any(a => a.FollowerId == followerId && a.FollowedId == userId && a.FolloweAcceptStatus == FolloweAcceptStatus.Accepted);
    }

    public void SetMutual(int userId, int followerId)
    {
        var userFollower = Get(a => a.FollowerId == followerId && a.FollowedId == userId);
        if (userFollower != null && userFollower.FolloweAcceptStatus == FolloweAcceptStatus.Accepted)
        {
            userFollower.IsMutual = true;
        }
    }
    public IQueryable<UserFollower> GetMyFollowersFollowers(User currentUser)
    {
       var followersFollowers = context.User.Where(c => c.Id == currentUser.Id)
            .SelectMany(c => c.Followees.Where(v => v.FolloweAcceptStatus == FolloweAcceptStatus.Accepted))
            .Select(x => x.Followed)
            .SelectMany(x => x.Followees.Where(m =>
                m.FolloweAcceptStatus == FolloweAcceptStatus.Accepted &&
                m.FollowedId != currentUser.Id &&
                //!m.Followed.Followees.Any(x => x.FollowedId == currentUser.Id)
                !m.Followed.Followers.Any(x => x.FolloweAcceptStatus != FolloweAcceptStatus.Rejected && x.FollowerId == currentUser.Id)
                ))
            .Include(x => x.Follower).OrderBy(_ => Guid.NewGuid()).ToList();
        return  followersFollowers.GroupBy(x => x.FollowerId).Select(x => x.FirstOrDefault()).AsQueryable();
    }

    public FollowInfoDto GetUserFollowInfo(int otherUserId, User currentUser)
    {   
        var myFolower = Context.UserFollower.Where(c => c.FollowedId == currentUser.Id && c.FollowerId == otherUserId).FirstOrDefault();
        var myFollowing = Context.UserFollower.Where(c => c.FollowerId == currentUser.Id && c.FollowedId == otherUserId).FirstOrDefault();

        var followInfoDto = new FollowInfoDto()
        {
            IsMyFollower = myFolower != null && myFolower.FolloweAcceptStatus == FolloweAcceptStatus.Accepted,
            IsMyFollowing = myFollowing != null && myFollowing.FolloweAcceptStatus == FolloweAcceptStatus.Accepted,
            Follower = myFolower,
            Following = myFollowing
        };
        return followInfoDto;
    }

    public UserFollower GetUserFollowerRejected(int followerId, int followedId)
    {
        return context.UserFollower.Where(a => a.FollowerId == followerId && a.FollowedId == followedId && a.FolloweAcceptStatus != FolloweAcceptStatus.Rejected).FirstOrDefault(); 
    }
    public UserFollower GetUserFollower(int followerId, int followedId)
    {
        return context.UserFollower.Where(a => a.FollowerId == followerId && a.FollowedId == followedId).FirstOrDefault();
    }
    public bool GetPendingFollwoRequest(int followerId, int followedId)
    {
        return context.UserFollower.Any(a => a.FollowerId == followerId &&
                       a.FollowedId == followedId &&
                       a.FolloweAcceptStatus == FolloweAcceptStatus.Pending);
    }
    public User GetFollowerUser(int followerId)
    { 
        return context.User.Where(x=> x.Id == followerId).FirstOrDefault();
    }

    public  UserFollower UpdateSeenStory(int id,int ownerId)
    {
        var userfollower = context.UserFollower.Where(x => x.FollowerId == ownerId && x.FollowedId == id).FirstOrDefault();
       

        if (userfollower != null)
        {
            userfollower.HasSeenStory = true;
            context.UserFollower.Update(userfollower);
            context.SaveChanges();
        }
        return userfollower;

    }

}

