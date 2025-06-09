using Aps.CommonBack.Base.Repositories;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class LikeArticleCommentReadRepository : Repository<LikeArticleComment, DataWriteContext>, ILikeArticleCommentReadRepository
{
    public LikeArticleCommentReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;

    public IQueryable<ArticleComment> GetArticleComments(int Id)
    {
        return context.ArticleComment.Where(x => x.Id == Id);
    }
    public IQueryable<LikeArticleComment> GetArticleCommentsByCommentId(int Id, int userid)
    {
        return context.LikeArticleComment.Where(x => x.ArticleCommentId == Id && x.UserId == userid).Include(d => d.ArticleComment).AsNoTracking();
    }
}
