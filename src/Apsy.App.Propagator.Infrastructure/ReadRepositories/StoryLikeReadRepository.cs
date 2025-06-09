


using Aps.CommonBack.Base.Repositories;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StoryLikeReadRepository
 : Repository<StoryLike, DataWriteContext>, IStoryLikeReadRepository
{
    public StoryLikeReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataWriteContext context;


    #endregion
    #region functions

    public IQueryable<StoryLike> GetStoryLikeByStoryAndUserId(int storyId, int userId)
    {
        return context.StoryLike.Where((StoryLike a) => a.StoryId == storyId && a.UserId == userId);
    }
    #endregion
}
