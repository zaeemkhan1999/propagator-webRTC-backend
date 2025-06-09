

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IStorySeenRepository
 : IRepository<StorySeen>
{

	#region functions
	StorySeen GetStorySeen(int? storyId, int? userId);
    List<StorySeenInput> GetStorySeenForAddSeens(List<StorySeenInput> input);
	IQueryable<StorySeen> GetStorySeens();
	#endregion
}
