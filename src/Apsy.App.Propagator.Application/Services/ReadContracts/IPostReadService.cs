using Apsy.App.Propagator.Domain.Common;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IPostReadService: IServiceBase<Post, PostInput>
    {
        ResponseBase<List<PostWatchHistory>> GetAllWatchedHistory();
        SingleResponseBase<PostDto> GetPost(int id);
        CustomListResponseBase<PostDto> GetPosts();
        ListResponseBase<PostDto> GetTopPosts(DateTime from);
        ListResponseBase<PostDto> GetFollowersPosts();
        ListResponseBase<Comment> GetComments(int postId);
        ListResponseBase<UserViewPost> GetViews();
        ListResponseBase<PostLikes> GetLikedPosts();
        ListResponseBase<SavePostDto> GetSavedPosts();
        ListResponseBase<PostDto> GetRandomPosts();
        Task<PostExploreDto> GetExploreRecommendedPostsAsync(int? lastId, int pageSize, string searchTerm, int skip, int take);
        Task<PostExploreDto> GetExploreRecommendedImagePostsAsync(int? lastId, int pageSize, string searchTerm, int skip, int take);
        Task<PostExploreDto> GetExploreRecommendedVideoPostsAsync(int? lastId, int pageSize, string searchTerm, int skip, int take);
        Task<PostExploreDto> GetExplorePostsAsync(int? lastId, int pageSize, string searchTerm);
        ListResponseBase<PostDto> GetAdsPosts();
        ListResponseBase<PostDto> GetPromotedPosts();
        Task<ListResponseBase<PostDto>> GetRecommendedPosts();
        IQueryable<PostDto> GetRecommendedPostsForHandler();
        ResponseBase<bool> CheckIsNewPostAvailable(DateTime from);

    }
}
