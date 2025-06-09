namespace Apsy.App.Propagator.Infrastructure.Repositories;
    public class ApplicationLogReadRepository
 : Repository<ApplicationLogs, DataWriteContext>, IApplicationLogReadRepository
{
        public ApplicationLogReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
        : base(dbContextFactory)
        {
            context = dbContextFactory.CreateDbContext();
        }

        #region props
        private DataWriteContext context;

        #endregion
        #region functions
        #endregion
    }

