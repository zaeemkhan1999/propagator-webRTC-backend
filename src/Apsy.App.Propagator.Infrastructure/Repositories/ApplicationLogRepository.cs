namespace Apsy.App.Propagator.Infrastructure.Repositories;
    public class ApplicationLogRepository
 : Repository<ApplicationLogs, DataReadContext>, IApplicationLogRepository
    {
        public ApplicationLogRepository(IDbContextFactory<DataReadContext> dbContextFactory)
        : base(dbContextFactory)
        {
            context = dbContextFactory.CreateDbContext();
        }

        #region props
        private DataReadContext context;

        #endregion
        #region functions
        #endregion
    }

