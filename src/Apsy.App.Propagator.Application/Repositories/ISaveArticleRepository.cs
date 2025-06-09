namespace Apsy.App.Propagator.Application.Repositories;

public interface  ISaveArticleRepository
 : IRepository<SaveArticle>
{

#region functions

    public SaveArticle GetSaveArticle(int Id,int UserId);
    public IQueryable<SaveArticleDto>  GetSavedArticles(User currentUser );
#endregion
}
