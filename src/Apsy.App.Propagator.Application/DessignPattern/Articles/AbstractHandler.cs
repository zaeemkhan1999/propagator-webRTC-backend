namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

// The default chaining behavior can be implemented inside a base handler
// class.
public class AbstractHandler : IHandler
{
    public AbstractHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private IHandler _nextHandler;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public IHandler SetNext(IHandler handler)
    {
        _nextHandler = handler;

        // Returning a handler from here will let us link handlers in a
        // convenient way like this:
        // monkey.SetNext(squirrel).SetNext(dog);
        return handler;
    }

    public virtual ListResponseBase<ArticleDto> Handle(object request, User currentUser)
    {
        if (_nextHandler != null)
        {
            return _nextHandler.Handle(request,currentUser);
        }
        else
        {
            return null;
        }
    }

    public  /*static*/ Expression<Func<Article, bool>> WhereList
    {
        get
        {
            var currentUser = GetCurrentUser();
            currentUser.NotInterestedArticleIds ??= new List<int>();

            return (article) => !article.User.Blocks.Any(c => c.BlockedId == currentUser.Id) && !article.Reports.Any(c => c.ReporterId == currentUser.Id) &&
                        !article.User.Blockers.Any(c => c.BlockerId == currentUser.Id) && article.isCreatedInGroup == false &&
                        (!article.User.PrivateAccount || article.User.Followers.Any(x => x.FollowerId == currentUser.Id)) &&
                          !currentUser.NotInterestedArticleIds.Any(x => article.Id == x) && article.User.IsDeletedAccount == false;
        }
    }



    //public static Expression<Func<Product, ResponseListProductDto>> MapList { get { return x => new ResponseListProductDto { Id = x.Id, Title = x.Title, Brand = x.Brand, Count = x.Count, CreateDate = x.CreateDate.ToShortDateString(), ModifyDate = x.ModifyDate.ToShortDateString(), IsActive = x.IsActive, Price = x.Price }; } }
    public  /*static*/ Expression<Func<Article, ArticleDto>> MapList
    {
        get
        {
            var currentUser = GetCurrentUser();
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

    public User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

}
