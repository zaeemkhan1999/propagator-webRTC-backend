


using Aps.CommonBack.Base.Repositories;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StoryLikeRepository
 : Repository<StoryLike, DataReadContext>, IStoryLikeRepository
{
    public StoryLikeRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataReadContext context;


    #endregion
    #region functions

    public IQueryable<StoryLike> GetStoryLikeByStoryAndUserId(int storyId, int userId)
    {
        return context.StoryLike.Where((StoryLike a) => a.StoryId == storyId && a.UserId == userId);
    }
    #endregion
}
