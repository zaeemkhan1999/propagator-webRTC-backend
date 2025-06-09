namespace Apsy.App.Propagator.Application.Services.ReadContracts;

public interface IHideStoryReadService : IServiceBase<HideStory, HideStoryInput>
{
    ListResponseBase<HideStoryDto> GetHideStories(User currentUsers);
}