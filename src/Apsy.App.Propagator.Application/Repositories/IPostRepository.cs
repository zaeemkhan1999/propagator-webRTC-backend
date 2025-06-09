namespace Apsy.App.Propagator.Application.Repositories;

public interface IPostRepository : IRepository<Post>
//IPostRepositoryBase<User, PostLikes, Post, Comment>
{
    IQueryable<Post> GetPostById(int id);
    IQueryable<PostDto> GetPostByUser(int id, int userId);
    IQueryable<PostDto> GetPosts(User currentUser);
    IQueryable<PostDto> GetAdsPosts(User currentUser);
    IQueryable<PostDto> GetPromotedPosts(User currentUser);
    IQueryable<PostDto> GetRandomPosts(User currentUser);
    IQueryable<Post> GetExplorePostsAsyncBaseQuery(int? id, User currentUser, string searchTerm);
    bool CheckIsNewPostAvailable(DateTime from);
    IQueryable<PostDto> GetTopPosts(int userId, DateTime from);
    IQueryable<PostDto> GetFollowersPosts(int userId);
    IQueryable<Ads> GetPostAds(int id);
    Post GetPostWithPoster(int id);
    IQueryable<SavePost> GetSavePost(int id, int userId);
    IQueryable<SavePostDto> GetSavedPost(int userId);
    IQueryable<PostLikes> GetPostLikesByUser(int userId);
    IQueryable<PostLikes> GetUserPostLikeWithPostDetails(int id, int userId);
    Task<List<Post>> GetPostsForAddViews(List<int> id, int userId);
    InterestedUser GetPostInterestedUser(int id, int userId, int posterId);
    int GetSinglePostLikesCount(int id);
    int GetPostLikesCount();
    int GetLikeCount(string content);
    int GetTagUsesCount(string content);
    int? GetPostTaskId(int id);
    Task<Post> UpdatePostsEngagement(Post post);
    Task UpdatePostsCompreessedResponse(string PostItemString, int PostId);
    Task UpdatePostsCompreessedTaskId(int TaskId, int PostId);
    int GetTotalUserPinPostCount(int userId);
    bool CheckIsPostActiveAds(int postId);
}