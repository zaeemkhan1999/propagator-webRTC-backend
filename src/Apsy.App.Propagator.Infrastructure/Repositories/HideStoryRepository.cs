namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class HideStoryRepository
 : Repository<HideStory, DataReadContext>, IHideStoryRepository
{
    public HideStoryRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataReadContext context;


    public IQueryable<HideStory> IsHideStory(int HiderId, int HidedID)
    {
        return context.HideStory.Where(x => x.HiderId == HiderId && x.HidedId == HidedID);
    }

    public IQueryable<HideStoryDto> GetHideStories(User user)
    {
       return context.UserFollower.Where(c => c.FollowedId == user.Id)
            .Select(c => new HideStoryDto
            {
                Follower = c.Follower,
                IsHided = c.Followed.HidedStory.Any(a => a.HidedId == c.FollowerId)
            });
    }

    
    #region functions

    public List<int> GetOtherUserIdsForhideStory(List<int> otherUserIds, int userId)
    {
        return otherUserIds.Where(c => !context.HideStory.Any(x => x.HidedId == c && x.HiderId == userId)).ToList();
    }

    public IQueryable<HideStory> GetHidedStoryForUser(int otherUserId, int userId)
    {
        return context.HideStory.Where((HideStory a) => a.HiderId == userId && a.HidedId == otherUserId);
    }

    public IQueryable<HideStory> GetHidedStoriesForUsers(List<int> otherUserIds, int userId)
    {
        return context.HideStory.Where(r => otherUserIds.Contains(r.HidedId) && r.HiderId == userId);
    }
    #endregion
    #endregion
}
