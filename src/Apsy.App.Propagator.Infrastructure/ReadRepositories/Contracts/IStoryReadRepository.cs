namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IStoryReadRepository
 : IRepository<Story>
{


	#region functions
	IQueryable<Story> GetStoryByUserId(int id);
    IQueryable<Settings> GetSettings();
    IQueryable<Story> GetStory();
    IQueryable<HideStory> GetHideStory();
    IQueryable<Story> GetStoryById(int entityId);
    DbSet<Story> GetStoryDbSet();
    IQueryable<User> GetUser();
    IQueryable<StoryLike> GetStoryLike();
    IQueryable<UserFollower> GetUserFollower();
    #endregion

	#region functions
	IQueryable<StoryDto> GetMyStories(int userId, int hoursGetStoryUser = 0);
	IQueryable<StoryDto> GetStoriesForAdmin();
	IQueryable<Story> GetMyStories(List<int> ids, int userId);
	IQueryable<Story> GetStories(int userId);
	IQueryable<Story> GetNotDeletedStories();
	int GetTotalStoriesCount();
	IQueryable<Story> GetStoriesById(int id, bool activeStory = false);
	IQueryable<Story> GetStoriesCount(int userId, bool activeStory = false);	
	bool IsStoryAvailable(int id);

	bool IsUserMyFollower(int otherUserId, int userId);
	#endregion

}
