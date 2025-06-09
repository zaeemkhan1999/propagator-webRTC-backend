namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IArticleCommentReadRepository
 : IRepository<ArticleComment>
{

    #region functions
    IQueryable<ArticleComment> GetAllArticleComment();

    public IQueryable<ArticleCommentDto> GetArticleComment(int Id, int UserId);
    public IQueryable<ArticleCommentDto> GetArticleComments(bool Loaddeleted, User CurrentUser);
    public ArticleComment UndoDeleteArticleComment(int Id);
    #endregion
}
