

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserLoginReadRepository
 : Repository<UserLogin,DataWriteContext>, IUserLoginReadRepository
{
public UserLoginReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
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
