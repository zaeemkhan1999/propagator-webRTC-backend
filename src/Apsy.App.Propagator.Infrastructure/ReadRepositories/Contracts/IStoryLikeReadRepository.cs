namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IStoryLikeReadRepository
 : IRepository<StoryLike>
{

#region functions
    IQueryable<StoryLike> GetStoryLikeByStoryAndUserId(int storyId, int userId);
#endregion
}
