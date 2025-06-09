namespace Apsy.App.Propagator.Application.Services;

public class StoryCommentService : ServiceBase<StoryComment, StoryCommentInput>, IStoryCommentService
{
    public StoryCommentService(IStoryCommentRepository repository, IHttpContextAccessor httpContextAccessor, IPublisher publisher, IStoryRepository storyRepository) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _storyRepository = storyRepository;
    }

    private readonly IStoryCommentRepository repository;
    private readonly IStoryRepository _storyRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;

    public async Task<ResponseBase<StoryComment>> AddStory(StoryCommentInput input, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.NotFound;
        if (input.UserId is null || input.UserId <= 0 || input.StoryId is null)
        {
            return ResponseStatus.NotEnoghData;
        }

        //repository.Where<Story>(x => x.Id == (int)input.StoryId)
        var story = _storyRepository.GetStoryById(input.StoryId ?? 0) 
            .Select(c => new
            {
                c.Id,
                c.UserId,
                c.User.CommentNotification,
            }).FirstOrDefault();


        if (story == null)
            return ResponseStatus.UserNotFound;

        var comment = input.Adapt<StoryComment>();
        var commentResult = repository.Add(comment);


        try
        {
            if (currentUser.Id != story.UserId && story.CommentNotification)
                await _publisher.Publish(new AddStoryCommentEvent(story.Id, currentUser.Id, story.UserId));
        }
        catch
        {
        }



        return new(commentResult);
    }
}