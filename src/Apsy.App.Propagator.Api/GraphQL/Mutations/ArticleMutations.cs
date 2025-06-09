


using Apsy.App.Propagator.Infrastructure.Redis;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class ArticleMutations
{
    [GraphQLName("article_createArticle")]
    public virtual ResponseBase<Article> Create(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticleService service, [Service] IRedisCacheService redisCache,
                        ArticleInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        input.ArticleType = ArticleType.RegularArticle;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            input.IsByAdmin = false;
        var response = service.Add(input);
        if (response != null)
        {    string myCacheKey = $"artical_Get123";
             redisCache.PublishUpdateAsync("cache_invalidation", myCacheKey);
        }
        return response;
    }

    [GraphQLName("article_promoteArticle")]
    public ResponseBase<ArticleAdsDto> PromoteArticle(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticleService service,
                        PromoteArticleInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.PromoteArticle(input, authentication.CurrentUser);
    }


    [GraphQLName("article_removeArticle")]
    public virtual ResponseStatus Remove(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IArticleService service, [Service] IRedisCacheService redisCache,
                        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var response = service.SoftDelete(entityId);
        if (response != null)
        {
            string myCacheKey = $"artical_Get123";
            redisCache.PublishUpdateAsync("cache_invalidation", myCacheKey);
        }
        return response;
    }

    [GraphQLName("article_updateArticle")]
    public ResponseBase<Article> Update(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] IArticleService service, [Service] IRedisCacheService redisCache,
                                ArticleInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        input.UserId = currentUser.Id;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            input.IsByAdmin = false;
        var response = service.Update(input);
        if (response != null)
        {
            string myCacheKey = $"artical_Get123";
            redisCache.PublishUpdateAsync("cache_invalidation", myCacheKey);
        }
        return response;
    }

    [GraphQLName("article_pinArticle")]
    public ResponseBase<Article> PinArticle(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IArticleService service,
        int articleId,
        bool pin)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.PinArticle(articleId, pin, authentication.CurrentUser);
    }


    [GraphQLName("article_verifyArticle")]
    public async Task<ResponseBase<Article>> VerifyArticle(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            //[Service] ITopicEventSender sender,
                            [Service] IArticleService service,
                            int articleId,
                            bool verify)
    {
        // var not = new Notification();
        // await sender.SendAsync($"{48}_NotificationAdded", not);
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.VerifyArticle(articleId, verify, authentication.CurrentUser);
    }

    [GraphQLName("article_likeArticle")]
    public async Task<ResponseBase<ArticleLike>> likeArticle(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IArticleService service,
        int articleId,
        bool liked = true)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;

        return await service.LikeArticle(currentUser.Id, articleId, liked, authentication.CurrentUser);
    }

    [GraphQLName("article_unLikeArticle")]
    public async Task<ResponseBase> UnLikeArticle(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IArticleService service,
        int articleId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return await service.UnLikeArticle(currentUser.Id, articleId);
    }

    [GraphQLName("article_addViewToArticle")]
    public async Task<ResponseBase<UserViewArticle>> AddView(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IArticleService service,
        int articleId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.AddView(articleId, authentication.CurrentUser);
    }

    [GraphQLName("article_saveArticle")]
    public async Task<ResponseBase> SavePost(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IArticleService service,
            int articleId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        return await service.SaveArticle(currentUser.Id, articleId);
    }

    [GraphQLName("article_unSaveArticle")]
    public async Task<ResponseBase> UnSaveArticle(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IArticleService service,
            int articleId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return await service.UnSavePost(currentUser.Id, articleId);
    }

    [GraphQLName("article_undoArticleRemove")]
    public async Task<ResponseStatus> UndoArticleRemove(
                  [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                  [Service] IArticleService service,
                  int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        if (authentication.CurrentUser.UserTypes != UserTypes.Admin && authentication.CurrentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.UndoDeleteArticle(entityId);
    }
}