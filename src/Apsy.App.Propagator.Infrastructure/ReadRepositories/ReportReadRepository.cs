

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ReportReadRepository : Repository<Report, DataWriteContext>, IReportReadRepository
{
    public ReportReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;
}