using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Infrastructure.Redis;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class StoryQueries
{

    [GraphQLName("story_getStoryUser")]
    public async Task<ListResponseBase<StoryUserDto>> Get(
             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
             [Service(ServiceKind.Default)] IStoryReadService service, [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"story_User{authentication.CurrentUser.Id}";
        var cacheStory = await redisCache.GetAsync<List<StoryUserDto>>(cacheKey);
        if (cacheStory !=null)
        {
            return ListResponseBase<StoryUserDto>.Success(cacheStory.AsQueryable());
        }

        var dbStory= service.GetStoryUser(authentication.CurrentUser);
        if (dbStory!=null)
        {
            await redisCache.SetAsync(cacheKey, dbStory.Result.ToList(), TimeSpan.FromMinutes(10));
        }
        return dbStory;
    }

    [GraphQLName("story_getMyStory")]
    public SingleResponseBase<Story> Get(
             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
             [Service(ServiceKind.Default)] IStoryReadService service,
             int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("story_getUserStoriesCount")]
    public async Task<ResponseBase<int>> GetUserStoriesCount(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] IStoryReadService service,
                                bool activeStory)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.GetUserStoriesCount(activeStory, authentication.CurrentUser);
    }

    [GraphQLName("story_getMyStories")]
    public ListResponseBase<StoryDto> GetMyStories(
     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
     [Service(ServiceKind.Default)] IStoryReadService service,
     [Service] IOpenSearchService _openSearchService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var searchResponse = service.GetStoryOpenSearch(authentication.CurrentUser);
        return searchResponse;

    }

    [GraphQLName("story_getStories")]
    public ListResponseBase<StoryDto> GetStories(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service(ServiceKind.Default)] IStoryService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetStories(false, authentication.CurrentUser);
    }


    private StoryDto ConvertToStoryDto(StoryDtoOpenSearch openSearchDto)
    {
        return new StoryDto
        {
            textPositionX = openSearchDto.textpositionx,
            textPositionY = openSearchDto.textpositiony,
            textStyle = openSearchDto.textstyle,
            Id = openSearchDto.id,
            CreatedDate = openSearchDto.createddate,
            UserId = openSearchDto.userid,
            ContentAddress = openSearchDto.contentaddress,
            StoryType = openSearchDto.storytype,
            Link = openSearchDto.link,
            Text = openSearchDto.text,
            LikedByCurrentUser = openSearchDto.likedbycurrentuser,
            SeenByCurrentUser = openSearchDto.seenbycurrentuser,
            HighLightId = openSearchDto.highlightid,
            PostId = openSearchDto.postid,
            ArticleId = openSearchDto.articleid,
            Duration = openSearchDto.duration,
            IsLiked = openSearchDto.isliked,
            LikeCount = openSearchDto.likecount,
            StorySeensCount = openSearchDto.storyseenscount,
            CommentCount = openSearchDto.commentcount,
            DeletedBy = openSearchDto.deletedby
        };
    }

    [GraphQLName("story_getStoriesForAdmin")]
    public ListResponseBase<StoryDto> GetStoriesForAdmin(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service(ServiceKind.Default)] IStoryReadService service)
    {
        //if (authentication.Status != ResponseStatus.Success)
        //{
        //    return authentication.Status;
        //}

        return service.GetStoriesForAdmin(authentication.CurrentUser);
    }

    [GraphQLName("post_getLikedStories")]
    public ListResponseBase<StoryLike> GetLikedPosts(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service] IStoryLikeService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return new(service.Get().Result.Where(c => c.Story.UserId == currentUser.Id));
    }
}