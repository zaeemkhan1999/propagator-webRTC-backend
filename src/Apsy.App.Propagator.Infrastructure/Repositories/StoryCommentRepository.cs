

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StoryCommentRepository
 : Repository<StoryComment,DataReadContext>,IStoryCommentRepository
{
public StoryCommentRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

#endregion
#region functions
#endregion
}
