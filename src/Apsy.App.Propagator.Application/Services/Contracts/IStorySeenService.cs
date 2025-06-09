

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IStorySeenService : IServiceBase<StorySeen, StorySeenInput>
{
    ListResponseBase<StorySeen> AddSeens(List<StorySeenInput> input);
}