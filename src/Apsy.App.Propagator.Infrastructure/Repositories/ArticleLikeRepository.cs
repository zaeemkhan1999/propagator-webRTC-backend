namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ArticleLikeRepository
 : Repository<ArticleLike, DataReadContext>, IArticleLikeRepository
{
public ArticleLikeRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

    public IQueryable<ArticleLike> GetAllArticleLike()
    {
        var query = context.ArticleLike.AsQueryable();
        return query;
    }
    public IQueryable<ArticleLikeDto> GetArticleLikes(User currentUser)
    {
        var ArticleLike = context.ArticleLike.Where(c => !c.Article.User.Blocks.Any(c => c.BlockedId == currentUser.Id) && c.UserId == currentUser.Id);
        var ArticleLikeDto = ArticleLike.Select(c => new ArticleLikeDto
            {
                Id = c.Id,
                CreatedDate = c.CreatedDate,
                Article = c.Article,
                //PostItems = JsonConvert.DeserializeObject<List<PostItem>>(c.PostItemsString).ToList(),
                ArticleItemsString = c.Article.ArticleItemsString,
                IsLiked = c.Article.ArticleLikes.Any(c => c.UserId == currentUser.Id),
                IsViewed = c.Article.UserViewArticles.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = c.Article.NotInterestedArticles.Any(c => c.UserId == currentUser.Id),
                IsSaved = c.Article.SaveArticles.Any(c => c.UserId == currentUser.Id),
                IsYourArticle = c.Article.UserId == currentUser.Id,
                CommentCount = c.Article.ArticleComments.Count,
                ShareCount = c.Article.Messages.Count,
                LikeCount = c.Article.ArticleLikes.Count,
                ViewCount = c.Article.UserViewArticles.Count,
            });
        return ArticleLikeDto;
    }

    public ArticleLike GetArticleLike(int Id,int UserId)
    {
        return context.ArticleLike.Where(x => x.ArticleId == Id && x.UserId == UserId).Include(x=>x.Article).FirstOrDefault();

    }

    #endregion
    #region functions
    #endregion
}
