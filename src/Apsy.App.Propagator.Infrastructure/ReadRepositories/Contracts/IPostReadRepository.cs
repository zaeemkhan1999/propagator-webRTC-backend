using Apsy.App.Propagator.Domain.Entities;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IPostReadRepository : IRepository<Post>
//IPostRepositoryBase<User, PostLikes, Post, Comment>
{
    Task<ResponseBase<PostWatchHistory>> AddWatchHistory(int id, int UserId);
    ResponseBase<List<PostWatchHistory>> GetWatchedHistory(int id);
    IQueryable<UserViewPost> GetAllUserViewPost();
    IQueryable<Post> GetAllPosts();
    IQueryable<Post> GetPostById(int id);
    IQueryable<PostDto> GetPostByUser(int id, int userId);
    IQueryable<PostDto> GetPosts(User currentUser);
    int GetPostCount(User currentUser);
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
    bool Ratelimiter(int UserId, string Type);
    User GetUserByPosterId(int UserId);
    IQueryable<PostDto> Explore(User currentUser,bool isRemoveAds);
    IQueryable<PostDto> ForYou(User currentUser, bool isRemoveAds);
    IQueryable<PostDto> MostEngaged(User currentUser, bool isRemoveAds);
    IQueryable<PostDto> MyPosts(User currentUser, bool isRemoveAds);
    IQueryable<PostDto> Newest(User currentUser, bool isRemoveAds);
    IQueryable<PostDto> News(User currentUser, bool isRemoveAds);
    IQueryable<PostDto> Recommended(User currentUser, bool isRemoveAds);
}