

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SavePostReadRepository
 : Repository<SavePost,DataWriteContext>, ISavePostReadRepository
{
public SavePostReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

    #endregion
    #region functions
    public IQueryable<SavePost> GetAllSavePost()
    {
        var query = context.SavePost.AsQueryable();
        return query;
    }
    #endregion
}
