namespace Apsy.App.Propagator.Application.Repositories;

public interface  IStoryLikeRepository
 : IRepository<StoryLike>
{

#region functions
    IQueryable<StoryLike> GetStoryLikeByStoryAndUserId(int storyId, int userId);
#endregion
}
