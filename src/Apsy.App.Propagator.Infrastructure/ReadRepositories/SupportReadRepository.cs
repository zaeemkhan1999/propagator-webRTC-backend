

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SupportReadRepository
 : Repository<Support,DataWriteContext>, ISupportReadRepository
{
public SupportReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
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
