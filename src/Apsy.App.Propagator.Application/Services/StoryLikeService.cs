namespace Apsy.App.Propagator.Application.Services;

public class StoryLikeService
 : ServiceBase<StoryLike,StoryLikeInput>,IStoryLikeService
{
public StoryLikeService (IStoryLikeRepository repository )
:base(repository)
{
		this.repository = repository;

}

#region props
	private IStoryLikeRepository repository;

#endregion
#region functions
#endregion
}
