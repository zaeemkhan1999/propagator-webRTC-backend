using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IStoryReadService : IServiceBase<Story, StoryInput>
    {
        ListResponseBase<StoryDto> GetStoryOpenSearch(User currentUser);
        ListResponseBase<StoryUserDto> GetStoryUser(User currentUser);
        ListResponseBase<StoryDto> GetStories(bool myStories, User currentUser);
        ListResponseBase<StoryDto> GetStoriesForAdmin(User currentUser);
        Task<ResponseBase<int>> GetUserStoriesCount(bool activeStory, User currentUser);
    }
}
