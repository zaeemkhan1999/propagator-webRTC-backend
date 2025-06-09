

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class VerificationRequestRepository
 : Repository<VerificationRequest,DataReadContext>,IVerificationRequestRepository
{
public VerificationRequestRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

#endregion
#region functions
	public IQueryable<VerificationRequest> GetVerificationRequest()
	{
		var query = context.VerificationRequest.AsQueryable();
		return query;

    }
    public async Task<VerificationRequest> GetVerificationRequestById(int requestId)
    {
        var query = await context.VerificationRequest.FindAsync(requestId);
        return query;
    }
    public IQueryable<User> GetUser()
    {
        var query = context.User.AsQueryable();
        return query;
    }
    #endregion
}
