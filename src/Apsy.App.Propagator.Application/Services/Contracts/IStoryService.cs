

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IStoryService : IServiceBase<Story, StoryInput>
{
    public void CheckStories(int userId);
    public void SetHasStory(int userId);
    public void ResetHasStory(int userId);
    Task<bool> redisStory(int currentUser);
    ResponseBase<bool> SoftDeleteAll(List<int> ids, User currentUser);
    ResponseBase<Story> AddSotyToHighLigh(int storyId, int highLightId,User currentUser);
    ResponseBase<Story> RemoveStoryFromHighLigh(int storyId, User currentUser);
    Task<ResponseBase<StoryLike>> LikeStory(int userId, int storyId, bool isLiked,User currentUser);
    ResponseBase UnLikeStory(int userId, int storyId);
    ListResponseBase<HideStory> HideStories(List<int> otherUserIds,User currentUser);
    ResponseBase<HideStory> HideStory(int otherUserId, User currentUser);
    ResponseBase UnHideStory(int otherUserId, User currentUser);
    ListResponseBase<HideStory> UnHideStories(List<int> otherUserIds, User currentUser);
    Task<ResponseStatus> UndoDeleteStore(int entityIdm);
    ListResponseBase<StoryDto> GetStories(bool myStories, User currentUser);
}