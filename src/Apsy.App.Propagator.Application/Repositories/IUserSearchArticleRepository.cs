namespace Apsy.App.Propagator.Application.Repositories;

public interface  IUserSearchArticleRepository
 : IRepository<UserSearchArticle>
{

    #region functions
    IQueryable<UserSearchArticle> GetUserSearchArticle();
#endregion
}
