using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read;

public class HideStoryReadService : ServiceBase<HideStory, HideStoryInput>, IHideStoryReadService
{
    public HideStoryReadService(IHideStoryReadRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IHideStoryReadRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ListResponseBase<HideStoryDto> GetHideStories(User currentUser)
    {
        var hideStories = repository.GetHideStories(currentUser);
        return new(hideStories);
    }

}
