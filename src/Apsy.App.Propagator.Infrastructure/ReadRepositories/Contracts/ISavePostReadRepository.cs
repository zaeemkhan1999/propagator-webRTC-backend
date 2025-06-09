namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISavePostReadRepository
 : IRepository<SavePost>
{

    #region functions
    IQueryable<SavePost> GetAllSavePost();
    #endregion
}
