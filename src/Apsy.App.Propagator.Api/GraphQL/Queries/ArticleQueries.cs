using Apsy.App.Propagator.Application.DessignPattern.Articles;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Infrastructure.Redis;
using Apsy.App.Propagator.Infrastructure.Repositories;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class ArticleQueries
{
    [GraphQLName("article_getArticle")]
    public SingleResponseBase<ArticleDto> GetArticle(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticalReadService service,
                        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetArticle(entityId,authentication.CurrentUser.Id);
    }

    [GraphQLName("article_getArticles")]
    public async Task<ListResponseBase<ArticleDto>> GetArticles(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticalReadService service, [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey=$"artical_Get123";
        var cacheArticals= await redisCache.GetAsync<List<ArticleDto>>(cacheKey);
        if (cacheArticals!=null)
        {
            return ListResponseBase<ArticleDto>.Success(cacheArticals.AsQueryable());
        }

        var dbArticals= service.GetArticles(authentication.CurrentUser);
        if (dbArticals != null) {
        await redisCache.SetAsync(cacheKey, dbArticals.Result.ToList(),TimeSpan.FromMinutes(10));
        }
        return dbArticals;
    }

    [GraphQLName("article_getArticlesInAdvanceWay")]
    public ListResponseBase<ArticleDto> GetPostsInAdvanceWay(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] IArticleRepository articleRepository,
                    [Service(ServiceKind.Default)] IHttpContextAccessor _httpAccessor,
                    GetArticleType getArticleType)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var explore = new ArticleExploreHandler(articleRepository, _httpAccessor);
        var forYou = new ArticleForYouHandler(articleRepository, _httpAccessor);
        var myArticles = new ArticleMyArticlesHandler(articleRepository, _httpAccessor);
        var newest = new ArticleNewestHandler(articleRepository, _httpAccessor);
        var news = new ArticleNewsHandler(articleRepository, _httpAccessor);
        var mostEngageds = new ArticleMostEngagedHandler(articleRepository, _httpAccessor);

        explore.SetNext(forYou).SetNext(myArticles).SetNext(newest).SetNext(news).SetNext(mostEngageds);
        return explore.Handle(getArticleType,authentication.CurrentUser);
    }


    [GraphQLName("post_getTopArticles")]
    public ListResponseBase<ArticleDto> GetTopArticles(
                   [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                   [Service(ServiceKind.Default)] IArticalReadService service,
                   DateTime from)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetTopArticles(from, authentication.CurrentUser.Id);
    }


    [GraphQLName("article_getFollowersArticles")]
    public ListResponseBase<ArticleDto> GetFollowersArticles(
                                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticalReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetFollowersArticles(authentication.CurrentUser.Id);
    }

    [GraphQLName("article_getLikedArticles")]
    public ListResponseBase<ArticleLikeDto> GetLikedArticles(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IArticleLikeReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetArticleLikes(authentication.CurrentUser);
    }

    [GraphQLName("article_getArticleViews")]
    public ListResponseBase<UserViewArticle> GetArticleViews(
                                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service] IArticalReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetViews();
    }

    [GraphQLName("article_getSavedArticles")]
    public ListResponseBase<SaveArticleDto> GetSavedArticles(
                                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service] IArticalReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetSavedArticles(authentication.CurrentUser);
    }
}