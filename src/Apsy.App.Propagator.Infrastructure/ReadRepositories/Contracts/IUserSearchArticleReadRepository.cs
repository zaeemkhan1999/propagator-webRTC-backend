namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserSearchArticleReadRepository
 : IRepository<UserSearchArticle>
{

    #region functions
    IQueryable<UserSearchArticle> GetUserSearchArticle();
#endregion
}
