namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ArticleCommentRepository
 : Repository<ArticleComment, DataReadContext>, IArticleCommentRepository
{
public ArticleCommentRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;



    #endregion
    #region functions
    public IQueryable<ArticleComment> GetAllArticleComment()
    {
        var query = context.ArticleComment.AsQueryable();
        return query;
    }
    public IQueryable<ArticleCommentDto> GetArticleComment(int id,int UserId)
    {
        return context.ArticleComment.Where(d => !d.Reports.Any(c => c.ReportedId == UserId))
            .Select(c => new ArticleCommentDto()
            {
                ArticleComment = c,
                IsLiked = c.LikeArticleComments.Any(c => c.UserId == UserId),
                LikeCount = c.LikeArticleComments.Count,
                HasChild = c.Children.Any(),
                ChildrenCount = c.Children.Count
            }).AsNoTracking().AsQueryable();
    }

    public IQueryable<ArticleCommentDto> GetArticleComments(bool loaddeleted, User currentUser)
    {
        return context.ArticleComment.Where(d => !d.Reports.Any(t => t.ReporterId == currentUser.Id))
            .Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Select(c => new ArticleCommentDto
            {
                ArticleComment = c,
                IsLiked = c.LikeArticleComments.Any(c => c.UserId == currentUser.Id),
                LikeCount = c.LikeArticleComments.Count,
                HasChild = c.Children.Any(),
                ChildrenCount = c.Children.Count
            }).AsNoTracking().AsQueryable();
    }

    public ArticleComment UndoDeleteArticleComment(int Id)
    {
        return context.ArticleComment.Where(c => c.Id == Id)
          .Include(x => x.Article)
          .ThenInclude(x => x.User)
          .FirstOrDefault();
    }
    #endregion
}
