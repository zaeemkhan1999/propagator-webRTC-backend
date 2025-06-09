using Aps.CommonBack.Base.Repositories;


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class InterestedReadUserRepository
 : Repository<InterestedUser, DataWriteContext>, IInterestedUserReadRepository
{
    public InterestedReadUserRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataWriteContext context;

    #endregion
    #region functions
    public IQueryable<InterestedUser> GetInterestedUsers(int Id)
    {
        return context.InterestedUser.Where(x => x.Id==Id);
    }
    #endregion
}
