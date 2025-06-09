using Apsy.App.Propagator.Infrastructure.Redis;
using Apsy.App.Propagator.Infrastructure.Repositories;
using System.Linq;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class PostMutations
{
    [GraphQLName("post_addWatchHistory")]
    public async Task<ResponseBase<List<PostWatchHistory>>> AddWatchHistory(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service] IPostService service,
         int[] postIds
         )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var result = await service.AddWatchHistory(postIds);


        return result;

    }
    [GraphQLName("post_createPost")]
    public async Task<ResponseBase<Post>> Create(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPostService service,    
        PostInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        input.PostType = PostType.RegularPost;
        input.PosterId = currentUser.Id;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            input.IsByAdmin = false;
        var added = await service.AddPost(input);
        if (added != null)
        {
            await service.redisPost(authentication.CurrentUser.Id);
        }
        return added;
    }

    [GraphQLName("post_createPostAds")]
    public ResponseBase<PostAdsDto> CreatePostAds(
       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IPostService service,
       PostAdsInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;

        input.PostType = PostType.Ads;
        input.PosterId = currentUser?.Id;
        return service.AddPostAds(input);
    }

    [GraphQLName("post_promotePost")]
    public ResponseBase<PostAdsDto> PromotePost(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPostService service,
        PromotePostInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.PromotePost(input);
    }

    [GraphQLName("post_removePost")]
    public ResponseStatus Remove(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] IPostService service,
                    int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        var isDeleted = service.SoftDelete(entityId);
        if (isDeleted!=null)
        {
            service.redisPost(authentication.CurrentUser.Id);
        }
        return isDeleted;
    }

    [GraphQLName("post_updatePost")]
    public ResponseBase<Post> Update(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPostService service,
        PostInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        input.PosterId = currentUser.Id;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            input.IsByAdmin = false;
        var updated= service.Update(input);
        if (updated != null)
        {
            service.redisPost(authentication.CurrentUser.Id);
        }
        return updated;
    }

    [GraphQLName("post_pinPost")]
    public ResponseBase<Post> PinPost(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPostService service,
        int postId,
        bool pin)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.PinPost(postId, pin);
    }

    [GraphQLName("post_likePost")]
    public async Task<ResponseBase<PostLikes>> LikePost(
              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
              [Service] IPostService service,
              int postId,
              bool liked = true)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return await service.LikePost(currentUser.Id, postId, liked);
    }

    [GraphQLName("post_unLikePost")]
    public async Task<ResponseBase> UnLikePost(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPostService postService,
        int postId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (!currentUser.IsActive) return CustomResponseStatus.UserIsNotActive;
        if (currentUser.IsDeletedAccount) return CustomResponseStatus.UserIsNotActive;

        return await postService.UnLikePost(currentUser.Id, postId);
    }

    [GraphQLName("post_addViewToPosts")]
    public async Task<ResponseBase<List<UserViewPost>>> AddView(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPostService service,
        int[] postIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.AddViews(postIds.ToList());
    }

    [GraphQLName("post_savePost")]
    public async Task<ResponseBase> SavePost(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service] IPostService service, [Service] IRedisCacheService redisCache,
                int postId,
                bool liked = true)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        string cacheKey = $"save_Posts{authentication.CurrentUser.Id}";
        var response=await service.SavePost(currentUser.Id, postId, liked);
        if (response!=null)
        {
            await redisCache.PublishUpdateAsync("cache_invalidation", cacheKey);
        }
        return response;

    }

    [GraphQLName("post_unSavePost")]
    public async Task<ResponseBase> UnSavePost(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IPostService service, [Service] IRedisCacheService redisCache,
            int postId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        string cacheKey = $"save_Posts{authentication.CurrentUser.Id}";
        var response= await service.UnSavePost(currentUser.Id, postId);
        if (response != null)
        {
            await redisCache.PublishUpdateAsync("cache_invalidation", cacheKey);
        }
        return response;
    }

    [GraphQLName("post_undoPostRemove")]
    public async Task<ResponseStatus> UndoPostRemove(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IPostService service,
            int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        if (authentication.CurrentUser.UserTypes != UserTypes.Admin && authentication.CurrentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.UndoDeletePost(entityId);
    }

}