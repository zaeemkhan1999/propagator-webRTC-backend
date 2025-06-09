

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ResetPasswordRequestReadRepository : Repository<ResetPasswordRequest, DataWriteContext>, IResetPasswordRequestReadRepository
{
    public ResetPasswordRequestReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) 
        : base(dbContextFactory)
    {
    }
}