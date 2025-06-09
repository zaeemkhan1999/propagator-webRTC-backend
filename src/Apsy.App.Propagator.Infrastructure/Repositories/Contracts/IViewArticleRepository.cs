namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IViewArticleRepository
 : IRepository<ViewArticle>
{

    #region functions
    IQueryable<ViewArticle> GetAllViewArticle();
    #endregion
}
