


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ViewPostReadRepository
 : Repository<UserViewPost, DataWriteContext>, IViewPostReadRepository
{
    public ViewPostReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataWriteContext context;

    #endregion
    #region functions

    public IQueryable<UserViewPost> GetPostViews()
    {
       return context.UserViewPost.AsNoTracking().AsQueryable();
    }

    #endregion
}
