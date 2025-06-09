namespace Apsy.App.Propagator.Application.Repositories;

public interface  ILikeCommentRepository
 : IRepository<LikeComment>
{
   IQueryable<LikeComment> GetLikeByCommentId(int Id, int UserId);
    int GetLikeComments(int Id);
    #region functions
    #endregion

}
