namespace Apsy.App.Propagator.Application.Repositories
{
    public interface IFollowerRepository : IRepository<UserFollower>
    {
        bool IsFollowed(int userId, int followerId);
        void SetMutual(int userId, int followerId);
        //void Follow(int userId, int followerId);
        IQueryable<UserFollower> GetMyFollowersFollowers(User currentUser);
        FollowInfoDto GetUserFollowInfo(int otherUserId, User currentUser);
        UserFollower GetUserFollower(int followerId,int followedId);
        UserFollower GetUserFollowerRejected(int followerId, int followedId);
        bool GetPendingFollwoRequest(int followerId, int followedId);
         User GetFollowerUser(int followerId);
    }
}