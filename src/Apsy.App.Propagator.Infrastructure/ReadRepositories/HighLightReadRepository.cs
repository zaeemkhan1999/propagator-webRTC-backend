

using Apsy.App.Propagator.Domain.Entities;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class HighLightReadRepository
 : Repository<HighLight,DataWriteContext>, IHighLightReadRepository
{
public HighLightReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

    #endregion
    #region functions
    public IQueryable<Story> GetStoriesByHighlightId(List<int> Ids, int UserId)
    {
        return context.Story.Where(x => Ids.Contains(x.Id) && x.UserId==UserId);
    }
    public IQueryable<HighLight> GetHighlightId(int Id)
    {
        return context.HighLight.Where(x => x.Id == Id);
    }
    #endregion
}
