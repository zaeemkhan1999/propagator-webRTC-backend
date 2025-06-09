namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SaveArticleRepository
 : Repository<SaveArticle, DataReadContext>, ISaveArticleRepository
{
public SaveArticleRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

    #endregion
    #region functions
    public IQueryable<SaveArticle> GetAllSaveArticle()
    {
        var query = context.SaveArticle.AsQueryable();
        return query;
    }
    public SaveArticle GetSaveArticle(int Id, int UserId)
    { 
		return context.SaveArticle.Where(a => a.ArticleId == Id && a.UserId == UserId).Include(c => c.Article).FirstOrDefault();
    }

    public IQueryable<SaveArticleDto> GetSavedArticles(User currentUser)
    {
        var baseQuery = context.SaveArticle.Where(c => c.UserId == currentUser.Id)
            .Where(c => !c.User.Blocks.Any(c => c.BlockedId == currentUser.Id))
            .Where(c => c.User.IsDeletedAccount == false);
          var query =  baseQuery.Select(c => new SaveArticleDto
            {
                Article = c.Article,
                ArticleId = c.ArticleId,
                User = c.User,
                UserId = c.UserId,
                IsLiked = c.Article.ArticleLikes.Any(c => c.UserId == currentUser.Id),
                IsViewed = c.Article.UserViewArticles.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = c.Article.NotInterestedArticles.Any(c => c.UserId == currentUser.Id),
                IsSaved = c.Article.SaveArticles.Any(c => c.UserId == currentUser.Id),
                IsYourArticle = c.UserId == currentUser.Id,
                CommentCount = c.Article.ArticleComments.Count,
                ShareCount = c.Article.Messages.Count,
                LikeCount = c.Article.ArticleLikes.Count,
                ViewCount = c.Article.UserViewArticles.Count,
            }).AsNoTracking().AsQueryable();
        return query;
    }

    #endregion
}
