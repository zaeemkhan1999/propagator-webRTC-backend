using Pipelines.Sockets.Unofficial.Buffers;

namespace Apsy.App.Propagator.Application.Services;

public class StorySeenService : ServiceBase<StorySeen, StorySeenInput>, IStorySeenService
{
    public StorySeenService(IStorySeenRepository repository, IHttpContextAccessor httpContextAccessor, IStoryRepository storyRepository, IUserRepository userRepository, IFollowerRepository followerRepository) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _storyRepository = storyRepository;
        _userRepository = userRepository;
        _followerRepository = followerRepository;
    }
    private readonly IFollowerRepository _followerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStorySeenRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStoryRepository _storyRepository;

    public override ResponseBase<StorySeen> Add(StorySeenInput input)
    {
        //if (!repository.Any<Story>(a => a.Id == input.StoryId))
        if (!_storyRepository.IsStoryAvailable((int)input.StoryId))
        {
            return ResponseStatus.NotFound;
        }

        //var storySeen = repository.Where<StorySeen>(a => a.StoryId == input.StoryId && a.UserId == input.UserId).FirstOrDefault();
        var storySeen = repository.GetStorySeen(input.StoryId, input.UserId);
        if (storySeen != null)
        {
            return storySeen;
        }
        var userstory = _storyRepository.GetStory().Where(x => x.UserId == input.OwnerId && x.DeletedBy == 0 && x.CreatedDate >= DateTime.Now.AddHours(-36)).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        if (userstory != null && userstory.Id == input.StoryId)
        {
            var createdStoryDate = userstory.CreatedDate;
            var elapsedTime = DateTime.Now - createdStoryDate;
            var remainingTime = TimeSpan.FromHours(36) - elapsedTime;
            _followerRepository.UpdateSeenStory(input.UserId, input.OwnerId);
            Task.Delay(remainingTime).ContinueWith(_ => ResetStorySeen(input.UserId, input.OwnerId));

        }

        StorySeen val = input.Adapt<StorySeen>();
        repository.Add(val);
        return new ResponseBase<StorySeen>(val);
    }
    public void ResetStorySeen(int userId, int OwnerId)
    {
        var user = _followerRepository.GetUserFollower( OwnerId, userId);
        if (user != null)
        {
            user.HasSeenStory = false;
            _userRepository.UpdateAsync(user);
        }
    }
    public ListResponseBase<StorySeen> AddSeens(List<StorySeenInput> input)
    {
        input = input.Distinct().ToList();
        //input = input.Where(c => !repository.GetDbSet<StorySeen>().Any(x => c.StoryId == x.StoryId && x.UserId == c.UserId)).ToList();
        //input = input.Where(c => !repository.GetStorySeens().Any(x => c.StoryId == x.StoryId && x.UserId == c.UserId)).ToList();
        input = repository.GetStorySeenForAddSeens(input);

        var val = input.Adapt<List<StorySeen>>();
        repository.AddRange(val);
        return new(val.AsQueryable());
    }
}