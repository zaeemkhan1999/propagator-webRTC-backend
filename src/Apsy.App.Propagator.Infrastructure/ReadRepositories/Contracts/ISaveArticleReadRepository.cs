namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISaveArticleReadRepository
 : IRepository<SaveArticle>
{

    #region functions
    IQueryable<SaveArticle> GetAllSaveArticle();
    public SaveArticle GetSaveArticle(int Id,int UserId);
    public IQueryable<SaveArticleDto>  GetSavedArticles(User currentUser );
#endregion
}
