

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StoryCommentReadRepository
 : Repository<StoryComment,DataWriteContext>, IStoryCommentReadRepository
{
public StoryCommentReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

#endregion
#region functions
#endregion
}
