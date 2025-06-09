using Apsy.App.Propagator.Domain.Common.Dtos.Dtos;

namespace Propagator.Common.Services.Contracts
{
    public interface IPostService : IServiceBase<Post, PostInput>
    {
        Task<bool> redisPost(int currentUser);
        Task<ResponseBase<List<PostWatchHistory>>>  AddWatchHistory(int[] ids);
        Task<ResponseBase<Post>> AddPost(PostInput input);
        ResponseBase<PostAdsDto> AddPostAds(PostAdsInput input);
        ResponseBase<PostAdsDto> PromotePost(PromotePostInput input);
        ResponseBase<Post> PinPost(int postId, bool pin);
        Task<ResponseBase<Comment>> CreateComment(CommentInput commentInput);
        Task<ResponseBase<List<UserViewPost>>> AddViews(List<int> postIds);
        Task<ResponseBase> SavePost(int userId, int postId, bool liked);
        Task<ResponseBase> UnSavePost(int userId, int postId);
        Task<ResponseBase<PostLikes>> LikePost(int userId, int postId, bool isLiked);
        Task<ResponseBase> UnLikePost(int userId, int postId);
        Task<ResponseStatus> UndoDeletePost(int entityId); 
        Task UpdatePostsCompressedResponse(TaskResquestModel resquestModel, int PostId);
    }
}