namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IHideStoryReadRepository
 : IRepository<HideStory>
{

    #region functions

    IQueryable<HideStory> IsHideStory(int HiderId, int HidedID);
    IQueryable<HideStoryDto> GetHideStories(User user);

    List<int> GetOtherUserIdsForhideStory(List<int> otherUserIds, int userId);
    IQueryable<HideStory> GetHidedStoryForUser(int otherUserId, int userId);
    IQueryable<HideStory> GetHidedStoriesForUsers(List<int> otherUserIds, int userId);

    #endregion
}
