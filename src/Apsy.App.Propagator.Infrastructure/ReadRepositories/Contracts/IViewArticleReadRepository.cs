namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IViewArticleReadRepository
 : IRepository<ViewArticle>
{

    #region functions
    IQueryable<ViewArticle> GetAllViewArticle();
    #endregion
}
