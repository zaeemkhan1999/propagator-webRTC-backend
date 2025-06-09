namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IArticleLikeRepository
 : IRepository<ArticleLike>
{
    IQueryable<ArticleLike> GetAllArticleLike();
    public IQueryable<ArticleLikeDto> GetArticleLikes(User currentUser);
    public ArticleLike GetArticleLike(int Id,int Userid);
#region functions
    #endregion
}
