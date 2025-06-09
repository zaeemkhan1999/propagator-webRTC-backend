using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class StorySeenReadService : ServiceBase<StorySeen, StorySeenInput>, IStorySeenReadService
    {
        private readonly IFollowerReadRepository _followerRepository;
        private readonly IUserReadRepository _userRepository;
        private readonly IStorySeenReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoryReadRepository _storyRepository;
        public StorySeenReadService(IStorySeenReadRepository repository, IHttpContextAccessor httpContextAccessor, IStoryReadRepository storyRepository, IUserReadRepository userRepository, IFollowerReadRepository followerRepository) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _storyRepository = storyRepository;
            _userRepository = userRepository;
            _followerRepository = followerRepository;
        }
        public ListResponseBase<StorySeenDto> GetStorySeens()
        {
            TypeAdapterConfig<StorySeen, StorySeenDto>
                    .NewConfig()
                    .Map(dest => dest.IsLiked, src => src.Story.StoryLikes.Any(x => x.UserId == src.UserId));

            //return new(repository.GetDbSet().ProjectToType<StorySeenDto>());
            return new(repository.GetStorySeens().ProjectToType<StorySeenDto>());
        }
    }
}
