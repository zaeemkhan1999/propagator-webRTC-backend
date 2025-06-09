namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StorySeenRepository
 : Repository<StorySeen, DataReadContext>, IStorySeenRepository
{
	public StorySeenRepository(IDbContextFactory<DataReadContext> dbContextFactory)
	: base(dbContextFactory)
	{
		context = dbContextFactory.CreateDbContext();
	}

	#region props
	private DataReadContext context;

	#endregion
	#region functions
	public StorySeen GetStorySeen(int? storyId, int? userId)
	{
		var storySeen = context.StorySeen.AsNoTracking().FirstOrDefault(a => a.StoryId == storyId && a.UserId == userId);
		return storySeen;
	}
	public List<StorySeenInput> GetStorySeenForAddSeens(List<StorySeenInput> input)
	{
        var storySeen = input.Where(c => !context.StorySeen.Any(x => c.StoryId == x.StoryId && x.UserId == c.UserId)).ToList();
        return storySeen;
    }

    public IQueryable<StorySeen> GetStorySeens()
	{
		var storySeen = context.StorySeen.AsNoTracking().AsQueryable();
		return storySeen;
	}
	#endregion
}
