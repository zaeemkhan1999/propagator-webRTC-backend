using Mapster;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StoryReadRepository
 : Repository<Story, DataWriteContext>, IStoryReadRepository
{
	public StoryReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
	: base(dbContextFactory)
	{
		context = dbContextFactory.CreateDbContext();
	}

	#region props
	private DataWriteContext context;


#endregion
#region functions
	public IQueryable<Settings> GetSettings()
	{
		var query = context.Settings.AsQueryable();
		return query;
	}

	public IQueryable<Story> GetStory()
	{
		var query = context.Story.AsQueryable();
		return query;
	}
	public IQueryable<User>GetUser()
	{
		var query = context.User.AsQueryable();
		return query;
	}
    public IQueryable<StoryLike> GetStoryLike()
    {
        var query = context.StoryLike.AsQueryable();
        return query;
    }
    public IQueryable<UserFollower> GetUserFollower()
    {
        var query = context.UserFollower.AsQueryable();
        return query;
    }
    public IQueryable<HideStory> GetHideStory()
	{
		var query = context.HideStory.AsQueryable();
		return query;
	}
    public IQueryable<Story> GetStoryById(int entityId)
    {
        var query = context.Story.Where(x => x.Id == entityId).AsQueryable();
		 
        return query;
    }
    public IQueryable<Story> GetStoryByUserId(int id)
    {
        var cutoffTime = DateTime.Now.AddHours(-36); 
        var stories = context.Story
            .Where(x => x.UserId == id && x.DeletedBy == 0 && x.CreatedDate >= cutoffTime)
            .AsQueryable();
        return stories;
    }

    public virtual DbSet<Story> GetStoryDbSet()
	{
		return context.Set<Story>(); 
	}

    #endregion
    #region functions
    public IQueryable<StoryDto> GetMyStories(int userId, int hoursGetStoryUser=0)
    {
        var story = context.Story.Where(d => d.DeletedBy == DeletedBy.NotDeleted)
                .Where(c => c.UserId == userId)
                .Where(x => x.CreatedDate > DateTime.UtcNow.AddHours(-hoursGetStoryUser))
                .ProjectToType<StoryDto>();
        return story;
    }
    public IQueryable<Story> GetMyStories(List<int> ids, int userId)
	{
		var story = context.Story.Where(r => ids.Contains(r.Id) && r.UserId == userId);
		return story;
	}
	public IQueryable<Story> GetStories(int userId)
	{
		var story = context.UserFollower.Where(c => c.FollowerId == userId && c.Follower.IsDeletedAccount == false)
			.SelectMany(c => c.Followed.Stories);
		return story;
	}
	public IQueryable<Story> GetStoriesById(int id, bool activeStory = false)
	{
		var story = context.Story.Where(c => c.Id == id);
		if (activeStory)
		{
			story = context.Story.IgnoreQueryFilters().Where(c => c.Id == id);
		}		
		return story;
	}
	public IQueryable<Story> GetStoriesCount(int userId, bool activeStory = false)
	{
		var story = context.Story.Where(x => x.UserId == userId && x.DeletedBy == DeletedBy.NotDeleted);
		if (activeStory)
			story.Where(c => c.CreatedDate.AddDays(1) > DateTime.UtcNow);

		return story;
	}
	public IQueryable<StoryDto> GetStoriesForAdmin()
	{
		var story = context.Story.ProjectToType<StoryDto>().AsNoTracking().AsQueryable();
		return story;
	}
	public IQueryable<Story> GetNotDeletedStories()
	{
		var story = context.Story.Where(d => d.DeletedBy == DeletedBy.NotDeleted).AsNoTracking().AsQueryable();
		return story;
	}

    public int GetTotalStoriesCount()
    {
        var story = context.Story.Where(d => d.DeletedBy == DeletedBy.NotDeleted).AsNoTracking().Count();
        return story;
    }

	public bool IsStoryAvailable(int id)
	{
		var isAvailable = context.Story.Any(x => x.Id == id);
		return isAvailable;
	}

    public bool IsUserMyFollower(int otherUserId, int userId)
    {
       return context.UserFollower.Any(a => a.FollowerId == otherUserId && a.FollowedId == userId);
    }

    #endregion
}
