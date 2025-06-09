

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ReportRepository : Repository<Report, DataReadContext>, IReportRepository
{
    public ReportRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;
}