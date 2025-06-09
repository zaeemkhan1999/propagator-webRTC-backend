namespace Apsy.App.Propagator.Application.Repositories;

public interface ILikeArticleCommentRepository : IRepository<LikeArticleComment>
{

    IQueryable<ArticleComment> GetArticleComments(int Id);
    IQueryable<LikeArticleComment> GetArticleCommentsByCommentId(int Id,int userid);
}