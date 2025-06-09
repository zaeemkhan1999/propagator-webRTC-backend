using Aps.CommonBack.Base.Repositories;
using Aps.CommonBack.Base.Repositories.Contracts;
using System.Linq;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class ArticleReadRepository : Repository<Article, DataWriteContext>, IArticleReadRepository
{
    public ArticleReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataWriteContext context;


    public async Task<Article> UpdateArticlesEngagement(Article article)
    {
        //DateTime startDateTime = DateTime.Today; //Today at 00:00:00
        DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);

        var savedCount = await Context.Set<SaveArticle>().Where(c => c.ArticleId == article.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var likedCount = await Context.Set<ArticleLike>().Where(c => c.ArticleId == article.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var commentsCount = await Context.Set<ArticleComment>().Where(c => c.ArticleId == article.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var hitsCount = await Context.Set<UserViewArticle>().Where(c => c.ArticleId == article.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var sharedCount = await Context.Set<Message>().Where(c => c.MessageType == MessageType.Article && c.ArticleId == article.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var notInterestedCount = await Context.Set<NotInterestedArticle>().Where(c => c.ArticleId == article.Id && c.CreatedDate > sevenDaysAgo).CountAsync();

        article.LatestUpdateThisWeeks = DateTime.UtcNow;
        article.ThisWeekSaveArticlesCount = savedCount;
        article.ThisWeekArticleLikesCount = likedCount;
        article.ThisWeekArticleCommentsCount = commentsCount;
        article.ThisWeekHits = hitsCount;
        article.ThisWeekShareCount = sharedCount;
        article.ThisWeekNotInterestedArticlesCount = notInterestedCount;

        return Update(article);
    }
    public IQueryable<Article> GetAllArticle()
    {
        var query = context.Article.AsQueryable();
        return query;
    }


    public ArticleDto GetArticle(int id, int UserId)
    {
        var Article = context.Article.Where(c => !c.User.Blocks.Any(c => c.BlockedId == UserId));
          var ArticleDto = Article.Select(c => new ArticleDto()
            {
                Article = c,
                ArticleItemsString = c.ArticleItemsString,
                IsLiked = c.ArticleLikes.Any(c => c.UserId == UserId),
                IsViewed = c.UserViewArticles.Any(c => c.UserId == UserId),
                IsNotInterested = c.NotInterestedArticles.Any(c => c.UserId == UserId),
                IsSaved = c.SaveArticles.Any(c => c.UserId == UserId),
                IsYourArticle = c.UserId == UserId,
                CommentCount = c.ArticleCommentsCount,
                ShareCount = c.ShareCount,
                LikeCount = c.ArticleLikesCount,
                ViewCount = c.Hits,
                NotInterestedArticlesCount = c.NotInterestedArticlesCount,
                ArticleComments = c.ArticleComments.OrderByDescending(c => c.LikeArticleComments.Count).Take(3)
                        .Select(c => new ArticleCommentDto
                        {
                            ArticleComment = c,
                            IsLiked = c.LikeArticleComments.Any(c => c.UserId == UserId),
                            HasChild = c.Children.Any(),
                            ChildrenCount = c.Children.Count,
                            LikeCount = c.LikeArticleComments.Count,
                        }).ToList()
            }).FirstOrDefault();
        return ArticleDto;
    }

    public IQueryable<ArticleDto> GetArticles(User currentUser)
    {
        var Article = context.Article.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Where(c => !c.User.Blocks.Any(c => c.BlockedId == currentUser.Id))
            .Where(d => !d.Reports.Any(c => c.ReporterId == currentUser.Id))
            .Where(d => d.User.IsDeletedAccount == false).Include(x => x.User);
          var ArticleDto = Article.Select(c => new ArticleDto
            {
                Article = c,
                ArticleItemsString = c.ArticleItemsString,
                IsLiked = c.ArticleLikes.Any(c => c.UserId == currentUser.Id),
                IsViewed = c.UserViewArticles.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = c.NotInterestedArticles.Any(c => c.UserId == currentUser.Id),
                IsSaved = c.SaveArticles.Any(c => c.UserId == currentUser.Id),
                IsYourArticle = c.UserId == currentUser.Id,
                CommentCount = c.ArticleCommentsCount,
                ShareCount = c.ShareCount,
                LikeCount = c.ArticleLikesCount,
                ViewCount = c.Hits,
                NotInterestedArticlesCount = c.NotInterestedArticlesCount,
                ArticleComments = c.ArticleComments.OrderByDescending(c => c.LikeArticleComments.Count).Take(3)
                        .Select(c => new ArticleCommentDto
                        {
                            ArticleComment = c,
                            IsLiked = c.LikeArticleComments.Any(c => c.UserId == currentUser.Id),
                            HasChild = c.Children.Any(),
                            ChildrenCount = c.Children.Count,
                            LikeCount = c.LikeArticleComments.Count,
                        }).ToList()
            }).AsNoTracking();
        return ArticleDto;
    }

    public IQueryable<Article> GetTopArticles(DateTime from, int UserId)
    {
        var baseQuery = context.Article.Where(c => !c.User.Blocks.Any(c => c.BlockedId == UserId))
            .Where(c => c.User.IsDeletedAccount == false);
            var query = baseQuery.Select(c => new ArticleDto
            {
                Article = c,
                ArticleItemsString = c.ArticleItemsString,
                IsLiked = c.ArticleLikes.Any(c => c.UserId == UserId),
                IsViewed = c.UserViewArticles.Any(c => c.UserId == UserId),
                IsNotInterested = c.NotInterestedArticles.Any(c => c.UserId == UserId),
                IsSaved = c.SaveArticles.Any(c => c.UserId == UserId),
                IsYourArticle = c.UserId == UserId,
                CommentCount = c.ArticleCommentsCount,
                ShareCount = c.ShareCount,
                LikeCount = c.ArticleLikesCount,
                ViewCount = c.Hits,
                NotInterestedArticlesCount = c.NotInterestedArticlesCount,
                ArticleComments = c.ArticleComments.OrderByDescending(c => c.LikeArticleComments.Count).Take(3)
                        .Select(c => new ArticleCommentDto
                        {
                            ArticleComment = c,
                            IsLiked = c.LikeArticleComments.Any(c => c.UserId == UserId),
                            HasChild = c.Children.Any(),
                            ChildrenCount = c.Children.Count,
                            LikeCount = c.LikeArticleComments.Count,
                        }).ToList()
            })
           .OrderBy(c => (1.2 * c.CommentCount) + c.ShareCount + c.LikeCount + c.ViewCount - c.NotInterestedArticlesCount);
        return (IQueryable<Article>)query;
    }
    public IQueryable<Article> GetFollowersArticles(int UserId)
    {
        var baseQuery = context.UserFollower
             .SelectMany(x => x.Follower.Articles);
                    var query = baseQuery.Select(c => new ArticleDto
                        {
                            Article = c,
                            ArticleItemsString = c.ArticleItemsString,
                            IsLiked = c.ArticleLikes.Any(c => c.UserId == UserId),
                            IsViewed = c.UserViewArticles.Any(c => c.UserId == UserId),
                            IsNotInterested = c.NotInterestedArticles.Any(c => c.UserId == UserId),
                            IsSaved = c.SaveArticles.Any(c => c.UserId == UserId),
                            IsYourArticle = c.UserId == UserId,
                            CommentCount = c.ArticleCommentsCount,
                            ShareCount = c.ShareCount,
                            LikeCount = c.ArticleLikesCount,
                            ViewCount = c.Hits,
                            NotInterestedArticlesCount = c.NotInterestedArticlesCount,
                            ArticleComments = c.ArticleComments.OrderByDescending(c => c.LikeArticleComments.Count).Take(3)
                                    .Select(c => new ArticleCommentDto
                                    {
                                        ArticleComment = c,
                                        IsLiked = c.LikeArticleComments.Any(c => c.UserId == UserId),
                                        HasChild = c.Children.Any(),
                                        ChildrenCount = c.Children.Count,
                                        LikeCount = c.LikeArticleComments.Count,
                                    }).ToList()
                        });
        return (IQueryable<Article>) query;
    }

    public Article GetArticle(int Id)
    {
        return context.Article.Include(x=> x.User).Where(x => x.Id == Id).FirstOrDefault();
    }

	public IQueryable<Article> GetArticleById(int id)
	{
		var article = context.Article.Where(p => p.Id == id).AsNoTracking().AsQueryable();
		return article;
	}

    public IQueryable<ArticleDto> GetNewestArticles(User currentUser)
    {
        var queryable = context.Article
               .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
              .Where(WhereList(currentUser))
             .OrderByDescending(c => c.CreatedDate);

        var result = queryable.Select(MapList(currentUser));
        return result;
    }
    public IQueryable<ArticleDto> GetNewsArticles(User currentUser)
    {
       var query =  context.Article
                       .Where(d => d.DeletedBy == DeletedBy.NotDeleted)
                       .Where(WhereList(currentUser))
                       .Where(c => c.IsByAdmin)
                       .Select(MapList(currentUser));
        return query;
    }
    public IQueryable<ArticleDto> MyArticles(User currentUser)
    {
        var query = context.Article
                         .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
                       .Where(WhereList(currentUser))
                       .Where(c => c.UserId == currentUser.Id)
                       .Select(MapList(currentUser));
        return query;
    }
    public IQueryable<ArticleDto> MostEngaged(User currentUser)
    {
        var queryable =context.Article.Where(WhereList(currentUser));

        //where is Show to me !
        queryable = queryable
            .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
            .Where(c => c.UserId != currentUser.Id)
                .Where(c =>
                    c.ArticleType == ArticleType.RegularArticle ||
                    c.IsPromote && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee))
                .OrderByDescending(c => c.LatestUpdateThisWeeks > DateTime.Today.AddDays(-7))
                .ThenByDescending(OrderList)
                .ThenByDescending(c => c.CreatedDate);

        return queryable.Select(MapList(currentUser));
    }
    public IQueryable<ArticleDto> ForYou(User currentUser)
    {
        var queryable = context.Article.Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin));

        queryable = queryable.Where(WhereList(currentUser));
        var iu = context.InterestedUser.Where(d => d.InterestedUserType == InterestedUserType.Article);
        var result = queryable.Select(article => new ArticleDto
        {
            Article = article,
            ArticleItemsString = article.ArticleItemsString,
            IsInterested = iu.Any(d => d.UserId == article.UserId),
            IsLiked = article.ArticleLikes.Any(c => c.UserId == currentUser.Id),
            IsViewed = article.UserViewArticles.Any(c => c.UserId == currentUser.Id),
            IsNotInterested = article.NotInterestedArticles.Any(c => c.UserId == currentUser.Id),
            IsSaved = article.SaveArticles.Any(c => c.UserId == currentUser.Id),
            IsYourArticle = article.UserId == currentUser.Id,
            CommentCount = article.ArticleCommentsCount,
            ShareCount = article.ShareCount,
            LikeCount = article.ArticleLikesCount,
            ViewCount = article.Hits,
            NotInterestedArticlesCount = article.NotInterestedArticlesCount,
            ArticleComments = article.ArticleComments.OrderByDescending(c => c.LikeCount).Take(3)
                .Select(c => new ArticleCommentDto
                {
                    ArticleComment = c,
                    IsLiked = c.LikeArticleComments.Any(c => c.UserId == currentUser.Id),
                    LikeCount = c.LikeArticleComments.Count,
                    HasChild = c.Children.Any(),
                    ChildrenCount = c.Children.Count
                }).ToList()
        }).OrderByDescending(d => d.IsInterested).ThenByDescending(d => d.Article.CreatedDate).ThenBy(d => Guid.NewGuid());
        return result;
    }
    public IQueryable<ArticleDto> Explore(User currentUser)
    {
        var queryable = context.Article.Where(WhereList(currentUser));
        //where is Show to me !
        queryable = queryable
            .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
            .Where(c => c.UserId != currentUser.Id)
            .Where(c => /*(c.ArticleType == ArticleType.Ads && c.PromoteOrAdsPriceScore > 0) ||*/
            c.ArticleType == ArticleType.RegularArticle ||
            c.IsPromote && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee))
            .OrderByDescending(c => c.LatestUpdateThisWeeks > DateTime.Today.AddDays(-7))
            .ThenByDescending(OrderList)
            .ThenByDescending(c => c.CreatedDate);

        var result = queryable.Select(MapList(currentUser));
        return result;
    }
    public bool Ratelimiter(int UserId, string Type)
    {
        var count = context.Article.Where(x => x.UserId == UserId && x.CreatedDate.Date == DateTime.UtcNow.Date).Count();
        var daylimit = context.RateLimit.Where(x => x.LimitType == Type).Select(x => x.LimitPrDay).FirstOrDefault();
        return count > daylimit;
    }
    public  /*static*/ Expression<Func<Article, bool>> WhereList(User currentUser)
    {
        
        
            
            currentUser.NotInterestedArticleIds ??= new List<int>();

            return (article) => !article.User.Blocks.Any(c => c.BlockedId == currentUser.Id) && !article.Reports.Any(c => c.ReporterId == currentUser.Id) &&
                        !article.User.Blockers.Any(c => c.BlockerId == currentUser.Id) && article.isCreatedInGroup == false &&
                        (!article.User.PrivateAccount || article.User.Followers.Any(x => x.FollowerId == currentUser.Id)) &&
                          !currentUser.NotInterestedArticleIds.Any(x => article.Id == x) && article.User.IsDeletedAccount == false;
        
    }



    //public static Expression<Func<Product, ResponseListProductDto>> MapList { get { return x => new ResponseListProductDto { Id = x.Id, Title = x.Title, Brand = x.Brand, Count = x.Count, CreateDate = x.CreateDate.ToShortDateString(), ModifyDate = x.ModifyDate.ToShortDateString(), IsActive = x.IsActive, Price = x.Price }; } }
    public  /*static*/ Expression<Func<Article, ArticleDto>> MapList(User currentUser)
    {
        
            return (article) => new ArticleDto()
            {
                Article = article,
                ArticleItemsString = article.ArticleItemsString,
                IsLiked = article.ArticleLikes.Any(c => c.UserId == currentUser.Id),
                IsViewed = article.UserViewArticles.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = article.NotInterestedArticles.Any(c => c.UserId == currentUser.Id),
                IsSaved = article.SaveArticles.Any(c => c.UserId == currentUser.Id),
                IsYourArticle = article.UserId == currentUser.Id,
                CommentCount = article.ArticleCommentsCount,
                ShareCount = article.ShareCount,
                LikeCount = article.ArticleLikesCount,
                ViewCount = article.Hits,
                NotInterestedArticlesCount = article.NotInterestedArticlesCount,
                ArticleComments = article.ArticleComments.OrderByDescending(c => c.LikeCount).Take(3)
                    .Select(c => new ArticleCommentDto
                    {
                        ArticleComment = c,
                        IsLiked = c.LikeArticleComments.Any(c => c.UserId == currentUser.Id),
                        LikeCount = c.LikeArticleComments.Count,
                        HasChild = c.Children.Any(),
                        ChildrenCount = c.Children.Count
                    }).ToList()
            };
        
    }

    

    public Expression<Func<Article, double>> OrderList
    {
        get
        {
            return (c) => /*c.PromoteOrAdsPriceScore +*/
                          (c.ThisWeekHits + c.ThisWeekSaveArticlesCount + c.ThisWeekArticleCommentsCount * 1.2 + c.ThisWeekArticleLikesCount + c.ThisWeekShareCount - c.ThisWeekNotInterestedArticlesCount) * 10 +
                         (c.Hits + c.SaveArticlesCount + c.ArticleCommentsCount * 1.2 + c.ArticleLikesCount + c.ShareCount - c.NotInterestedArticlesCount);
        }
    }
}