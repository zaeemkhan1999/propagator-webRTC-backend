namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IUserSearchArticleRepository
 : IRepository<UserSearchArticle>
{

    #region functions
    IQueryable<UserSearchArticle> GetUserSearchArticle();
#endregion
}
