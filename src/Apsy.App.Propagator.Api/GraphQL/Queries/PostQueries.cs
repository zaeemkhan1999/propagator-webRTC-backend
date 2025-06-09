
using System;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common;
using Apsy.App.Propagator.Domain.Common.Dtos;
using Apsy.App.Propagator.Infrastructure.Redis;
using Apsy.App.Propagator.Infrastructure.Repositories;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PostQueries
{
    //[Authorize(Policy = Permissions.AdReports)]
    //[Authorize(Policy = Permissions.SuspendAds)]

    // [GraphQLName("post_GetWatchedHistory")]
    // public ResponseBase<List<PostWatchHistory>> GetWatchedHistory(
    // [Authentication] Authentication authentication,
    // [Service(ServiceKind.Default)] IPostReadService service)
    //     {
    //         if (authentication.Status != ResponseStatus.Success)
    //         {
    //             return new ResponseBase<List<PostWatchHistory>>(authentication.Status);
    //         }

    //         var watchedPost = service.GetAllWatchedHistory();

    //         if (watchedPost == null)
    //         {
    //             return new ResponseBase<List<PostWatchHistory>>([]);
    //         }
    //         var result = watchedPost.Result
    //             .OrderByDescending(watch => watch.WatchDate)
    //             .Take(30)
    //             .ToList();
    //         return result;
    //     }
    [GraphQLName("post_GetWatchedHistory")]
    public async Task<ResponseBase<List<PostDto>>> GetWatchedHistory(
            [Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IPostReadRepository postRepository,
            [Service(ServiceKind.Default)] IHttpContextAccessor _httpAccessor,
            [Service(ServiceKind.Default)] IUsersSubscriptionReadService _usersSubscriptionService,
            [Service] IPostReadService postService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        var getPostType = GetPostType.Recommended;
        var watchedPost = postService.GetAllWatchedHistory();
        var filteredWatchPost = watchedPost.Result
                    .OrderByDescending(post => post.WatchDate)
                    //.Take(30)
                    .ToList() ?? [];

        var explore = new PostExploreHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var forYou = new PostForYouHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var myPosts = new PostMyPostsHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var newest = new PostNewestHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var news = new PostNewsHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var mostEngageds = new PostMostEngagedHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var postRecommended = new PostRecommendedHandler(postService, postRepository, _usersSubscriptionService, _httpAccessor);

        explore.SetNext(forYou).SetNext(myPosts).SetNext(newest).SetNext(news).SetNext(mostEngageds).SetNext(postRecommended);
        var dbPosts = await Task.Run(() => explore.Handle(getPostType, authentication.CurrentUser));

        var result = dbPosts.Result?
            //.OrderByDescending(post => post.Post.CreatedDate)
            //.Take(30)
            .ToList() ?? [];


        var watchDateLookup = filteredWatchPost.ToLookup(post => post.Post.Id, post => post.WatchDate);

        // Add WatchDate to result list
        foreach (var post in result)
        {
            if (watchDateLookup.Contains(post.Post.Id))
            {
                post.WatchDate = watchDateLookup[post.Post.Id].FirstOrDefault();// Use the first available WatchDate
            }
        }

        return result
                .OrderByDescending(x => x.WatchDate)
                .Take(30).ToList() ?? [];
    }
    [GraphQLName("post_getPost")]
    public SingleResponseBase<PostDto> GetPost(
                   [Authentication] Authentication authentication,
                   [Service(ServiceKind.Default)] IPostReadService service,
                   int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetPost(entityId);
    }

    [GraphQLName("post_getPosts")]
    public async Task<CustomListResponseBase<PostDto>> GetPosts(
                        [Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IPostReadService service, [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"post_Key{authentication.CurrentUser.Id}";
        //    var cachePosts = await redisCache.GetAsync<List<PostDto>>(cacheKey);
        //if (cachePosts != null)
        //{
        //    return ListResponseBase<PostDto>.Success(cachePosts.AsQueryable());
        //}
        var dbPosts= service.GetPosts();
        if (dbPosts!=null)
        {
            await redisCache.SetAsync(cacheKey, dbPosts.Result.ToList(), TimeSpan.FromMinutes(10));
        }
        return dbPosts;
    }

    [GraphQLName("post_getAds")]
    public ListResponseBase<PostDto> GetAds(
        [Authentication] Authentication authentication,
        [Service] IPostReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetAdsPosts();
    }

    [GraphQLName("post_getPromotedPosts")]
    public ListResponseBase<PostDto> GetPromotedPosts(
        [Authentication] Authentication authentication,
        [Service] IPostReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetPromotedPosts();
    }

    [GraphQLName("post_getRandomPosts")]
    public ListResponseBase<PostDto> GetRandomPosts(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPostReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetRandomPosts();
    }

    [GraphQLName("post_getExplorePosts")]
    public async Task<PostExploreDto> GetExplorePostsAsync(
           [Authentication] Authentication authentication,
           [Service(ServiceKind.Default)] IPostReadService service,
           int? lastId,
           int pageSize,
           string searchTerm, int skip, int take)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.GetExploreRecommendedPostsAsync(lastId, pageSize, searchTerm, skip, take);
    }

    [GraphQLName("post_getExploreImagePosts")]
    public async Task<PostExploreDto> GetExploreImagePostsAsync(
           [Authentication] Authentication authentication,
           [Service(ServiceKind.Default)] IPostReadService service,
           int? lastId,
           int pageSize,
           string searchTerm, int skip, int take)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.GetExploreRecommendedImagePostsAsync(lastId, pageSize, searchTerm, skip, take);
    }

    [GraphQLName("post_getExploreVideoPosts")]
    public async Task<PostExploreDto> GetExploreVideoPostsAsync(
           [Authentication] Authentication authentication,
           [Service(ServiceKind.Default)] IPostReadService service,
           int? lastId,
           int pageSize,
           string searchTerm, int skip, int take)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.GetExploreRecommendedVideoPostsAsync(lastId, pageSize, searchTerm, skip, take);
    }

    [GraphQLName("post_getPostsInAdvanceWay")]
    public async Task<ListResponseBase<PostDto>> GetPostsInAdvanceWay(
               [Authentication] Authentication authentication,
               [Service(ServiceKind.Default)] IPostReadRepository postRepository,
               [Service(ServiceKind.Default)] IHttpContextAccessor _httpAccessor,
               [Service(ServiceKind.Default)] IUsersSubscriptionReadService _usersSubscriptionService,
               [Service] IPostReadService postService,
               [Service] IRedisCacheService redisCache,
               GetPostType getPostType)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Advance_Post{authentication.CurrentUser.Id}";
        //var cachePosts = await redisCache.GetAsync<List<PostDto>>(cacheKey);
        //if (cachePosts != null)
        //{
        //    return ListResponseBase<PostDto>.Success(cachePosts.AsQueryable());
        //}

        var explore = new PostExploreHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var forYou = new PostForYouHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var myPosts = new PostMyPostsHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var newest = new PostNewestHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var news = new PostNewsHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var mostEngageds = new PostMostEngagedHandler(postRepository, _usersSubscriptionService, _httpAccessor);
        var postRecommended = new PostRecommendedHandler(postService, postRepository, _usersSubscriptionService, _httpAccessor);

        explore.SetNext(forYou).SetNext(myPosts).SetNext(newest).SetNext(news).SetNext(mostEngageds).SetNext(postRecommended);
        var dbPosts = await Task.Run(() => explore.Handle(getPostType, authentication.CurrentUser));
        if (dbPosts != null && dbPosts.Result != null)
       {
            //await redisCache.SetAsync(cacheKey, dbPosts.Result.ToList(), TimeSpan.FromMinutes(10));
            var postsList = dbPosts.Result.ToList();
            postsList = postsList
                .Select((post, index) =>
                {
                    if ((index + 1) % 5 == 0)
                    {
                        post.needAds = true;
                    }
                    return post;
                })
                .ToList();

            return ListResponseBase<PostDto>.Success(postsList.AsQueryable());
        }
        return dbPosts;
    }

    [GraphQLName("post_checkIsNewPostAvailable")]
    public ResponseBase<bool> CheckIsNewPostsAvailable(
                  [Authentication] Authentication authentication,
                  [Service(ServiceKind.Default)] IPostReadService service,
                  DateTime from)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.CheckIsNewPostAvailable(from);
    }


    [GraphQLName("post_getTopPosts")]
    public ListResponseBase<PostDto> GetTopPosts(
                   [Authentication] Authentication authentication,
                   [Service(ServiceKind.Default)] IPostReadService service,
                   DateTime from)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetTopPosts(from);
    }

    [GraphQLName("post_getFollowersPosts")]
    public async Task<ListResponseBase<PostDto>> GetFollowersPosts(
                        [Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IPostReadService service, [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"followers_Posts{authentication.CurrentUser.Id}";
        var cachePosts = await redisCache.GetAsync<List<PostDto>>(cacheKey);
        if (cachePosts != null)
        {
            return ListResponseBase<PostDto>.Success(cachePosts.AsQueryable());
        }
        var dbPosts= service.GetFollowersPosts();
        if (dbPosts != null)
        {
            await redisCache.SetAsync(cacheKey, dbPosts.Result.ToList(), TimeSpan.FromMinutes(10));
        }
        return dbPosts;
    }

    [GraphQLName("post_getLikedPosts")]
    public ListResponseBase<PostLikesDto> GetLikedPosts(
                [Authentication] Authentication authentication,
                [Service] IPostLikeReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetPostLikes();
    }

    [GraphQLName("post_getLikedPostsUsers")]
    public ListResponseBase<PostLikesDto> getLikedPostsUsers(
                                              [Authentication] Authentication authentication,
                         [Service] IPostLikeReadService service,
                         int postId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetPostLikesUsers(postId);
    }


    [GraphQLName("post_getPostViews")]
    public ListResponseBase<UserViewPost> GetPostViews(
                [Authentication] Authentication authentication,
                [Service] IPostReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetViews();
    }

    [GraphQLName("post_getSavedPosts")]
    public async Task<ListResponseBase<SavePostDto>> GetSavedPosts(
                              [Authentication] Authentication authentication,
                              [Service] IPostReadService service,
                              [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"save_Posts{authentication.CurrentUser.Id}";

        var cachePosts = await redisCache.GetAsync<List<SavePostDto>>(cacheKey);
        if (cachePosts != null)
        {
            return ListResponseBase<SavePostDto>.Success(cachePosts.AsQueryable());
        }

        var dbPosts = service.GetSavedPosts();
        if (dbPosts != null)
        {
            await redisCache.SetAsync(cacheKey, dbPosts.Result.ToList(), TimeSpan.FromMinutes(10));
        }
        return dbPosts;
    }
    [GraphQLName("post_getRecommendedPosts")]
    public async Task<ListResponseBase<PostDto>> GetRecommendedPosts(
                             [Authentication] Authentication authentication,
                             [Service] IPostReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.GetRecommendedPosts();
    }
    
}