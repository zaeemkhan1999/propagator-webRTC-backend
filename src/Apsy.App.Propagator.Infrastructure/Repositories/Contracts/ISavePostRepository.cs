namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  ISavePostRepository
 : IRepository<SavePost>
{

    #region functions
    IQueryable<SavePost> GetAllSavePost();
    #endregion
}
