namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ILikeCommentReadRepository
 : IRepository<LikeComment>
{
   IQueryable<LikeComment> GetLikeByCommentId(int Id, int UserId);
    int GetLikeComments(int Id);
    #region functions
    #endregion

}
