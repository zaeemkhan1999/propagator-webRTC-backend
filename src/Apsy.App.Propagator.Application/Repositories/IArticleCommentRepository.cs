namespace Apsy.App.Propagator.Application.Repositories;

public interface  IArticleCommentRepository
 : IRepository<ArticleComment>
{

    #region functions

    public IQueryable<ArticleCommentDto> GetArticleComment(int Id, int UserId);
    public IQueryable<ArticleCommentDto> GetArticleComments(bool Loaddeleted, User CurrentUser);
    public ArticleComment UndoDeleteArticleComment(int Id);
    #endregion
}
