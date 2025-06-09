

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SavePostRepository
 : Repository<SavePost,DataReadContext>,ISavePostRepository
{
public SavePostRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

    #endregion
    #region functions
    public IQueryable<SavePost> GetAllSavePost()
    {
        var query = context.SavePost.AsQueryable();
        return query;
    }
    #endregion
}
