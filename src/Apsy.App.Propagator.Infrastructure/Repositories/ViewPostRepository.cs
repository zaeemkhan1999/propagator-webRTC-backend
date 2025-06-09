


namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ViewPostRepository
 : Repository<UserViewPost, DataReadContext>, IViewPostRepository
{
    public ViewPostRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataReadContext context;

    #endregion
    #region functions

    public IQueryable<UserViewPost> GetPostViews()
    {
       return context.UserViewPost.AsNoTracking().AsQueryable();
    }

    #endregion
}
