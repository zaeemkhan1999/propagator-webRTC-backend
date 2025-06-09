using Aps.CommonBack.Base.Repositories;


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class InterestedUserRepository
 : Repository<InterestedUser, DataReadContext>, IInterestedUserRepository
{
    public InterestedUserRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataReadContext context;

    #endregion
    #region functions
    public IQueryable<InterestedUser> GetInterestedUsers(int Id)
    {
        return context.InterestedUser.Where(x => x.Id==Id);
    }
    #endregion
}
