

using Apsy.App.Propagator.Domain.Entities;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class HighLightRepository
 : Repository<HighLight,DataReadContext>,IHighLightRepository
{
public HighLightRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

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
