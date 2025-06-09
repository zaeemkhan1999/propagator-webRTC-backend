namespace Apsy.App.Propagator.Application.Services;

public class HighLightService : ServiceBase<HighLight, HighLightInput>, IHighLightService
{
    public HighLightService(
        IHighLightRepository repository,
        IStoryRepository storyRepository,
        IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _storyRepository = storyRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IHighLightRepository repository;
    private readonly IStoryRepository _storyRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResponseBase<HighLight> Add(HighLightInput input,User currentUser)
    {

        HighLight highlight = input.Adapt<HighLight>();
        repository.Add(highlight);

        if (input.StoryIds != null)
        {
            var stories = repository.GetStoriesByHighlightId(input.StoryIds,currentUser.Id).ToList();

            foreach (var item in stories)
            {
                item.HighLightId = highlight.Id;
            }
            _storyRepository.UpdateRange(stories);
        }
        return highlight;
    }



    public  ResponseBase<HighLight> Update(HighLightInput input,User currentUser)
    {
    
        HighLight val = input.Adapt<HighLight>();
        HighLight highLightFromDb = repository.GetHighlightId(val.Id).FirstOrDefault();
        if (highLightFromDb == null)
            return ResponseBase<HighLight>.Failure(ResponseStatus.NotFound);

        if (highLightFromDb.UserId != currentUser.Id)
            return ResponseStatus.NotFound;

        // to do Get from user stories
        var myStories = _storyRepository.Where<Story>(r => r.UserId == currentUser.Id).ToList();
        var stories = myStories.Where(r => r.HighLightId == highLightFromDb.Id && !input.StoryIds.Contains(r.Id)).ToList();
        foreach (var item in stories)
        {
            item.HighLightId = null;
        }
        _storyRepository.UpdateRange(stories);
        // to do Get from user stories
        var newStories = _storyRepository.Where<Story>(r => input.StoryIds.Contains(r.Id)).ToList();


        foreach (var item in newStories)
        {
            item.HighLightId = highLightFromDb.Id;
        }
        _storyRepository.UpdateRange(newStories);
        input.Adapt(highLightFromDb);
        _storyRepository.Update(highLightFromDb);

        return highLightFromDb;
    }

        
    public  ResponseStatus SoftDelete(int entityId,User currentUser)
    {
        HighLight byId = repository.GetById(entityId, checkDeleted: true);
        if (byId == null)
        {
            return ResponseStatus.NotFound;
        }

        if (byId.UserId != currentUser.Id)
        {
            return ResponseStatus.NotAllowd;
        }

        if (byId.IsDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        HighLight val = repository.Remove(byId);
        if (val.IsDeleted)
        {
            return ResponseStatus.Success;
        }

        return ResponseStatus.Failed;
    }

}