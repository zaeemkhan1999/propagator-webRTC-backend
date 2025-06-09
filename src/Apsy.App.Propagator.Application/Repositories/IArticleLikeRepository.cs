namespace Apsy.App.Propagator.Application.Repositories;

public interface  IArticleLikeRepository
 : IRepository<ArticleLike>
{
    public IQueryable<ArticleLikeDto> GetArticleLikes(User currentUser);
    public ArticleLike GetArticleLike(int Id,int Userid);
#region functions
    #endregion
}
