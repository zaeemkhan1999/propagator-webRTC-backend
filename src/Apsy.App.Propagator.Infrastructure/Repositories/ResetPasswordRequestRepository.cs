

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ResetPasswordRequestRepository : Repository<ResetPasswordRequest, DataReadContext>, IResetPasswordRequestRepository
{
    public ResetPasswordRequestRepository(IDbContextFactory<DataReadContext> dbContextFactory) 
        : base(dbContextFactory)
    {
    }
}