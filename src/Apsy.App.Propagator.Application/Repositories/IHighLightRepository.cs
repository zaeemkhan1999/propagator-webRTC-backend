namespace Apsy.App.Propagator.Application.Repositories;

public interface  IHighLightRepository
 : IRepository<HighLight>
{

#region functions
    IQueryable<Story> GetStoriesByHighlightId(List<int> Ids,int UserId);
    IQueryable<HighLight> GetHighlightId(int Id);
    #endregion
}
