using Apsy.App.Propagator.Infrastructure.Redis;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Apsy.App.Propagator.Application.Services;

public class StoryService : ServiceBase<Story, StoryInput>, IStoryService
{
    public StoryService(
        IRedisCacheService redisCache,
        IStoryRepository repository,
        IUserRepository userRepository,
        IHideStoryRepository hideStoryRepository,
        IStoryLikeRepository storyLikerepository,
        ISettingsRepository settingRepository,
          IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher) : base(repository)
    {
        _redisCache = redisCache;
        this.repository = repository;
        _userRepository = userRepository;
        _storyLikerepository = storyLikerepository;
        _hideStoryRepository = hideStoryRepository;
        _httpContextAccessor = httpContextAccessor;
        this.settingRepository = settingRepository;
        _publisher = publisher;
        _configuration = configuration;
    }
    private readonly IRedisCacheService _redisCache;
    private readonly IStoryRepository repository;
    private readonly IHideStoryRepository _hideStoryRepository;
    private readonly IStoryLikeRepository _storyLikerepository;
    private readonly IUserRepository _userRepository;
    private readonly ISettingsRepository settingRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;
    public override ResponseBase<Story> Add(StoryInput input)
    {
        switch (input.StoryType)
        {
            case StoryType.Post when input.PostId is null:
                return CustomResponseStatus.PostIsRequired;
            case StoryType.Article when input.ArticleId is null:
                return CustomResponseStatus.ArticleIsRequired;
            case StoryType.Video or StoryType.Image when input.ContentAddress is null:
                return CustomResponseStatus.ContentAddressIsRequired;
            case StoryType.Video when input.Duration is null:
                return CustomResponseStatus.VideoDurationIsRequired;
            case StoryType.Text when input.Text is null:
                return CustomResponseStatus.TextIsRequired;
        }

        var settings = settingRepository.GetFirstSettings(); //repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            //settings.TotalStoriesCount = repository.GetDbSet().Count(d => d.DeletedBy == DeletedBy.NotDeleted);
            settings.TotalStoriesCount = repository.GetTotalStoriesCount();
            repository.Update(settings);
        }

        return base.Add(input);
    }
    #region updateStory
    public override ResponseBase<Story> Update(StoryInput input)
    {

        switch (input.StoryType)
        {
            case StoryType.Post when input.PostId is null:
                return CustomResponseStatus.PostIsRequired;
            case StoryType.Article when input.ArticleId is null:
                return CustomResponseStatus.ArticleIsRequired;
            case StoryType.Video or StoryType.Image when input.ContentAddress is null:
                return CustomResponseStatus.ContentAddressIsRequired;
            case StoryType.Video when input.Duration is null:
                return CustomResponseStatus.VideoDurationIsRequired;
            case StoryType.Text when input.Text is null:
                return CustomResponseStatus.TextIsRequired;
        }
        var existingStory = repository.GetStoryById(input.Id.Value).FirstOrDefault();
        if (existingStory == null)
        {
            throw new ArgumentException("Story is null");
        }
        existingStory.StoryType = input.StoryType;
        existingStory.PostId = input.PostId != null ? input.PostId : existingStory.PostId;
        existingStory.ArticleId = input.ArticleId != null ? input.ArticleId : existingStory.ArticleId;
        existingStory.ContentAddress = input.ContentAddress != null ? input.ContentAddress : existingStory.ContentAddress;
        existingStory.Duration = input.Duration != null ? input.Duration : existingStory.Duration;
        existingStory.textPositionX = input.textPositionX != null ? input.textPositionX : existingStory.textPositionX;
        existingStory.textPositionY = input.textPositionY != null ? input.textPositionY : existingStory.textPositionY;
        existingStory.textStyle = input.textStyle != null ? input.textStyle : existingStory.textStyle;
        existingStory.Text = input.Text != null ? input.Text : existingStory.Text;


        var settings = settingRepository.GetFirstSettings();
        if (settings != null)
        {
            settings.TotalStoriesCount = repository.GetTotalStoriesCount();
            repository.Update(settings);
        }
        repository.Update(existingStory);

        return base.Update(input);
    }

    #endregion

    public override ResponseStatus SoftDelete(int entityId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        //Story storybyId = repository.GetById(entityId, checkDeleted: true);
        Story storybyId = repository.GetStoriesById(entityId, true).FirstOrDefault();
        if (storybyId == null)
        {
            return ResponseStatus.NotFound;
        }

        if (storybyId.UserId != currentUser.Id)
        {
            return ResponseStatus.NotAllowd;
        }

        if (storybyId.DeletedBy != DeletedBy.NotDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        //Story removedStory = repository.Remove(storybyId);
        storybyId.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;
        var removedStory = repository.Update(storybyId);

        if (removedStory.DeletedBy == DeletedBy.NotDeleted)
        {
            return ResponseStatus.Failed;
        }

        var settings = settingRepository.GetFirstSettings();// repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            //settings.TotalStoriesCount = repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).Count();
            settings.TotalStoriesCount = repository.GetTotalStoriesCount();
            repository.Update(settings);
        }

        return ResponseStatus.Success;
    }

    public ResponseBase<bool> SoftDeleteAll(List<int> ids, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        //var myStories = repository.Where(r => ids.Contains(r.Id) && r.UserId == currentUser.Id);
        var myStories = repository.GetMyStories(ids, currentUser.Id);
        var myStoriesCount = myStories.Count();
        repository.RemoveRange(myStories);

        var settings = settingRepository.GetFirstSettings();// repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            //settings.TotalStoriesCount = repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).Count();
            settings.TotalStoriesCount = repository.GetTotalStoriesCount();
            repository.Update(settings);
        }

        return true;
    }

    public ResponseBase<Story> AddSotyToHighLigh(int storyId, int highLightId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        if (!repository.Any<HighLight>(a => a.Id == highLightId))
        {
            return ResponseStatus.NotFound;
        }

        var storyFromDb = repository.GetStory().Where(a => a.Id == storyId).FirstOrDefault();
        //var storyFromDb = repository.Where<Story>(a => a.Id == storyId).FirstOrDefault();
        // var storyFromDb = repository.GetStoriesById(storyId).FirstOrDefault();
        if (storyFromDb is null)
            return ResponseStatus.NotFound;
        if (storyFromDb.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        storyFromDb.HighLightId = highLightId;
        return repository.Update(storyFromDb);
    }

    public ResponseBase<Story> RemoveStoryFromHighLigh(int storyId, User currentUser)
    {
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        if (!repository.GetStory().Any(a => a.Id == storyId))
            //if (!repository.Any<Story>(a => a.Id == storyId))
            if (!repository.GetStoriesById(storyId).Any())
            {
                return ResponseStatus.NotFound;
            }

        var storyFromDb = repository.GetStory().Where(a => a.Id == storyId).FirstOrDefault();
        //var storyFromDb = repository.Where<Story>(a => a.Id == storyId).FirstOrDefault();
        // var storyFromDb = repository.GetStoriesById(storyId).FirstOrDefault();
        if (storyFromDb is null)
            return ResponseStatus.NotFound;
        if (storyFromDb.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        storyFromDb.HighLightId = null;
        return repository.Update(storyFromDb);
    }

    public async Task<ResponseBase<StoryLike>> LikeStory(int userId, int storyId, bool isLiked, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var story = repository.GetStory().Where(a => a.Id == storyId).FirstOrDefault();
        //var story = repository.Where((Story a) => a.Id == storyId).FirstOrDefault();
        //  var story = repository.GetStoriesById(storyId).FirstOrDefault();

        if (story == null)
        {
            return ResponseStatus.NotFound;
        }

        if (!repository.GetUser().Any(a => a.Id == userId))
        {
            return ResponseStatus.UserNotFound;
        }

        // StoryLike storyLike = repository.GetStoryLike().Where(a => a.StoryId == storyId && a.UserId == userId).FirstOrDefault();
        StoryLike storyLike = _storyLikerepository.GetStoryLikeByStoryAndUserId(storyId, userId).FirstOrDefault();// repository.Where((StoryLike a) => a.StoryId == storyId && a.UserId == userId).FirstOrDefault();
        if (storyLike == null)
        {
            StoryLike entity = new StoryLike
            {
                UserId = userId,
                StoryId = storyId,
                Liked = isLiked
            };
            storyLike = repository.Add(entity);
        }
        else
        {
            storyLike.Liked = isLiked;
            return Update(storyLike);
        }

        try
        {
            if (currentUser.Id != story.UserId)
                await _publisher.Publish(new LikeStoryEvent(storyLike.Id, currentUser.Id, story.UserId));
        }
        catch
        {
        }

        return storyLike;
    }

    public ResponseBase UnLikeStory(int userId, int storyId)
    {
        StoryLike val = repository.GetStoryLike().Where(a => a.StoryId == storyId && a.UserId == userId).FirstOrDefault();
        //StoryLike val = _storyLikerepository.GetStoryLikeByStoryAndUserId(storyId, userId).FirstOrDefault(); //repository.Where((StoryLike a) => a.StoryId == storyId && a.UserId == userId).FirstOrDefault();

        if (val == null)
            return ResponseStatus.NotFound;
        else
            repository.Remove(val);

        return ResponseBase.Success();
    }


    public ListResponseBase<HideStory> HideStories(List<int> otherUserIds, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        otherUserIds = _hideStoryRepository.GetOtherUserIdsForhideStory(otherUserIds, currentUser.Id);// otherUserIds.Where(c => !repository.GetDbSet<HideStory>().Any(x => x.HidedId == c && x.HiderId == currentUser.Id)).ToList();
        List<HideStory> hideStories = new List<HideStory>();
        foreach (var item in otherUserIds)
        {
            hideStories.Add(new HideStory
            {
                HiderId = currentUser.Id,
                HidedId = item,
            });
        }

        repository.AddRange(hideStories);

        return new(hideStories.AsQueryable());
    }

    public ResponseBase<HideStory> HideStory(int otherUserId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        if (!repository.GetUser().Any(a => a.Id == otherUserId))
            //if (!repository.Any((User a) => a.Id == otherUserId))
            if (!_userRepository.isUserAvailable(otherUserId).Result)
            {
                return ResponseStatus.UserNotFound;
            }

        // var isMyFollower = repository.GetUserFollower().Any(a => a.FollowerId == otherUserId && a.FollowedId == currentUser.Id);
        var isMyFollower = repository.IsUserMyFollower(otherUserId, currentUser.Id); //repository.Any<UserFollower>(a => a.FollowerId == otherUserId && a.FollowedId == currentUser.Id);
        if (!isMyFollower)
            return CustomResponseStatus.UserIsNotYourFollower;

        HideStory val = repository.GetHideStory().Where(a => a.HiderId == currentUser.Id && a.HidedId == otherUserId).FirstOrDefault();
        // HideStory val = _hideStoryRepository.GetHidedStoryForUser(otherUserId, currentUser.Id).FirstOrDefault();//repository.Where((HideStory a) => a.HiderId == currentUser.Id && a.HidedId == otherUserId).FirstOrDefault();
        if (val != null)
            return ResponseStatus.AlreadyExists;
        HideStory entity = new HideStory
        {
            HiderId = currentUser.Id,
            HidedId = otherUserId,
        };
        return repository.Add(entity);
    }

    public ResponseBase UnHideStory(int otherUserId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        HideStory val = repository.GetHideStory().Where(a => a.HiderId == currentUser.Id && a.HidedId == otherUserId).FirstOrDefault();
        //HideStory val = _hideStoryRepository.GetHidedStoryForUser(otherUserId, currentUser.Id).FirstOrDefault();// repository.Where<HideStory>(a => a.HiderId == currentUser.Id && a.HidedId == otherUserId).FirstOrDefault();

        if (val == null)
            return ResponseStatus.NotFound;
        else
            repository.Remove(val);

        return ResponseBase.Success();
    }

    public ListResponseBase<HideStory> UnHideStories(List<int> otherUserIds, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var myHidedUsers = repository.GetHideStory().Where(r => otherUserIds.Contains(r.HidedId) && r.HiderId == currentUser.Id);
        //  var myHidedUsers = _hideStoryRepository.GetHidedStoriesForUsers(otherUserIds, currentUser.Id); //repository.Where<HideStory>(r => otherUserIds.Contains(r.HidedId) && r.HiderId == currentUser.Id);
        repository.RemoveRange(myHidedUsers);
        return new(myHidedUsers);
    }

    private User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

    public async Task<ResponseStatus> UndoDeleteStore(int entityId)
    {

        // var storybyId = repository.GetStoryById(entityId).FirstOrDefault();

        //Story storybyId = repository.GetById(entityId, checkDeleted: true);
        Story storybyId = repository.GetStoriesById(entityId, true).FirstOrDefault();
        if (storybyId == null)
        {
            return ResponseStatus.NotFound;
        }


        if (storybyId.DeletedBy == DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyUndo;
        }

        //Story removedStory = repository.Remove(storybyId);
        storybyId.DeletedBy = DeletedBy.NotDeleted;
        await repository.UpdateAsync(storybyId);

        var settings = settingRepository.GetFirstSettings();// repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            //settings.TotalStoriesCount = repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).Count();
            settings.TotalStoriesCount = repository.GetTotalStoriesCount();
            await repository.UpdateAsync(settings);
        }

        return ResponseStatus.Success;
    }
    public async Task<bool> redisStory(int currentUser)
    {
        var follower = _userRepository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != currentUser) && c.FollowedId == currentUser && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).ToList();
        var followersId = follower.Select(f => f.FollowerId).ToList();
        string myCacheKey = $"story_User{currentUser}";
        await _redisCache.PublishUpdateAsync("cache_invalidation", myCacheKey);

        if (followersId != null)
        {
            foreach (var varitem in followersId)
            {
                string cacheKey = $"story_User{varitem}";
                await _redisCache.PublishUpdateAsync("cache_invalidation", cacheKey);
            }
            return true;
        }
        return false;
    }
    public void SetHasStory(int userId)
    {
        var user = _userRepository.GetUserByIdAsync(userId);
        if (user != null)
        {
            user.HasStory = true;
            _userRepository.UpdateAsync(user);

            Task.Delay(TimeSpan.FromHours(36)).ContinueWith(_ => ResetHasStory(userId));
        }
    }
    public void ResetHasStory(int userId)
    {
        var user = _userRepository.GetUserByIdAsync(userId);
        if (user != null)
        {
            user.HasStory = false;
            _userRepository.UpdateAsync(user);
        }
    }
    public void CheckStories(int userId)
    {
        var userStories = repository.GetStoryByUserId(userId);
        if (userStories.Any())
        {
            var latestStory = userStories.OrderByDescending(s => s.CreatedDate).FirstOrDefault();
            var createdStoryDate = latestStory.CreatedDate;
            var elapsedTime = DateTime.Now - createdStoryDate;
            var remainingTime = TimeSpan.FromHours(36) - elapsedTime;

            if (remainingTime <= TimeSpan.Zero)
            {
                remainingTime = TimeSpan.Zero;
            }

            var user = _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.HasStory = true;
                _userRepository.UpdateAsync(user);

                Task.Delay(remainingTime).ContinueWith(_ => ResetHasStory(userId));
            }
            
        }
        else
        {
            ResetHasStory(userId);
        }

    }

    public ListResponseBase<StoryDto> GetStories(bool myStories, User currentUser)
    {

        // if (currentUser == null)
        //     return ResponseStatus.AuthenticationFailed;
        // setMapsterConfig(currentUser);

        if (myStories)
        {
            //var res = repository
            //   // .Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            //    .Where(d => d.DeletedBy == DeletedBy.NotDeleted)
            //    .Where(c => c.UserId == currentUser.Id)
            //    .ProjectToType<StoryDto>();
            var res = repository.GetMyStories(currentUser.Id);

            return new ListResponseBase<StoryDto>(res);
        }

        //var stories = repository.Where<UserFollower>(c => c.FollowerId == currentUser.Id && c.Follower.IsDeletedAccount == false)
        //    .SelectMany(c => c.Followed.Stories);
        var stories = repository.GetStories(currentUser.Id);
        var result = stories.ProjectToType<StoryDto>();

        return new ListResponseBase<StoryDto>(result);
    }

}