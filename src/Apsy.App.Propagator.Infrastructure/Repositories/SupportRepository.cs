

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SupportRepository
 : Repository<Support,DataReadContext>,ISupportRepository
{
public SupportRepository (IDbContextFactory<DataReadContext> dbContextFactory )
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
