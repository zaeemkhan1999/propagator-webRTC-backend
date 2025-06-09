

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserLoginRepository
 : Repository<UserLogin,DataReadContext>, IUserLoginRepository
{
public UserLoginRepository(IDbContextFactory<DataReadContext> dbContextFactory )
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
