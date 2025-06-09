using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Infrastructure.Redis;
using Stripe;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class StoryReadService: ServiceBase<Story, StoryInput>, IStoryReadService
    {
        private readonly IOpenSearchService _openSearchService;
        private readonly IRedisCacheService _redisCache;
        private readonly IStoryReadRepository repository;
        private readonly IHideStoryReadRepository _hideStoryRepository;
        private readonly IStoryLikeReadRepository _storyLikerepository;
        private readonly IUserReadRepository _userRepository;
        private readonly ISettingsReadRepository settingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublisher _publisher;
        private readonly IConfiguration _configuration;
        public StoryReadService(
            IOpenSearchService openSearchService,
       IRedisCacheService redisCache,
       IStoryReadRepository repository,
       IUserReadRepository userRepository,
       IHideStoryReadRepository hideStoryRepository,
       IStoryLikeReadRepository storyLikerepository,
       ISettingsReadRepository settingRepository,
         IConfiguration configuration,
       IHttpContextAccessor httpContextAccessor,
       IPublisher publisher) : base(repository)
        {
            _openSearchService = openSearchService;
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
        public ListResponseBase<StoryUserDto> GetStoryUser(User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var baseQuery = repository
                            .GetUserFollower().Where(c =>
                                        c.FollowerId == currentUser.Id && c.Followed.HidedStory.All(d => d.HidedId != currentUser.Id) &&
                                        c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted
                                        && c.Followed.Stories.Any(x => x.CreatedDate > DateTime.UtcNow.AddHours(-HoursGetStoryUser))
                                        && c.Followed.Stories.Any(x => x.DeletedBy == 0)
                                        && c.IsDeleted == false
                                        && c.Follower.IsDeletedAccount == false);
            var query = baseQuery.Select(c => new StoryUserDto()
            {
                HasNotSeenedStory =
                             c.Followed
                             .Stories
                             .Any(c => c.DeletedBy == DeletedBy.NotDeleted && c.IsDeleted == false && c.CreatedDate.AddDays(1.5) > DateTime.UtcNow && !c.StorySeens.Any(x => x.UserId == currentUser.Id)),

                StoryCount = c.Followed.Stories
                                             .Count(c => c.DeletedBy == DeletedBy.NotDeleted && c.IsDeleted == false && c.CreatedDate.AddDays(1.5) > DateTime.UtcNow),


                SeenedStoryCount = c.Followed.Stories
                                             .Count(c => c.DeletedBy == DeletedBy.NotDeleted && c.IsDeleted == false && c.CreatedDate.AddDays(1.5) > DateTime.UtcNow &&
                                                                         c.StorySeens.Any(x => x.UserId == currentUser.Id)),

                Stories = c.Followed.Stories
                             .Where(x => x.DeletedBy == DeletedBy.NotDeleted && x.IsDeleted == false && x.CreatedDate.AddDays(1.5) > DateTime.UtcNow)
                              .Select(story => new Story
                              {
                                  textPositionX = story.textPositionX, // Updated field
                                  textPositionY = story.textPositionY, // Updated field
                                  textStyle = story.textStyle,          // Updated field
                                  Id = story.Id,
                                  ContentAddress = story.ContentAddress,
                                  Text = story.Text,
                                  Link = story.Link,
                                  StoryType = story.StoryType,
                                  Duration = story.Duration,
                                  CreatedDate = story.CreatedDate,
                                  UserId = story.UserId,
                                  User = story.User,
                                  DeletedBy = story.DeletedBy,
                                  HighLight = story.HighLight,
                                  HighLightId = story.HighLightId,
                                  Post = story.Post,
                                  PostId = story.PostId,
                                  Article = story.Article,
                                  ArticleId = story.ArticleId,
                                  StoryComments = story.StoryComments,
                                  StoryLikes = story.StoryLikes,
                                  StorySeens = story.StorySeens,
                                  LikedByCurrentUser = story.StoryLikes.Any(like => like.UserId == currentUser.Id && like.Liked),
                                  SeenByCurrentUser = story.StorySeens.Any(seen => seen.UserId == currentUser.Id),
                                  Messages = story.Messages,
                                  Notifications = story.Notifications
                              })
                             .ToList(),

                StoryOwner = c.Followed
            });

            return new ListResponseBase<StoryUserDto>(query);
        }
        public async Task<ResponseBase<int>> GetUserStoriesCount(bool activeStory, User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var storyQueryAble = repository.GetStory().Where(x => x.UserId == currentUser.Id && x.DeletedBy == DeletedBy.NotDeleted);
            //var storyQueryAble = repository.Where(x => x.UserId == currentUser.Id && x.DeletedBy==DeletedBy.NotDeleted);
            //var storyQueryAble = repository.GetStoriesCount(currentUser.Id);
            if (activeStory)
                //storyQueryAble = storyQueryAble.Where(c => c.CreatedDate.AddDays(1) > DateTime.UtcNow);
                storyQueryAble = repository.GetStoriesCount(currentUser.Id, true);

            return await storyQueryAble.CountAsync();
        }
        public ListResponseBase<StoryDto>GetStoryOpenSearch(User currentUser)
        {
           
            var searchResponse = _openSearchService.Search<StoryDtoOpenSearch>("story", "");

            if (searchResponse.Documents.Any())
            {
                
                // Convert StoryDtoOpenSearch to StoryDto
                var stories = searchResponse.Hits
                    .Select(x => ConvertToStoryDto(x.Source))
                    .Where(u => u.UserId == currentUser.Id)
                    .Where(a => a.DeletedBy == DeletedBy.NotDeleted)
                    .Where(t => t.CreatedDate >= DateTime.Now.AddHours(-36))
                    .ToList();
                if(stories != null)
                setMapsterConfig(currentUser);

                return ListResponseBase<StoryDto>.Success(stories.AsQueryable());
            }
            return GetStories(true, currentUser);

        }
        private StoryDto ConvertToStoryDto(StoryDtoOpenSearch openSearchDto)
        {
            return new StoryDto
            {
                textPositionX = openSearchDto.textpositionx,
                textPositionY = openSearchDto.textpositiony,
                textStyle = openSearchDto.textstyle,
                Id = openSearchDto.id,
                CreatedDate = openSearchDto.createddate,
                UserId = openSearchDto.userid,
                ContentAddress = openSearchDto.contentaddress,
                StoryType = openSearchDto.storytype,
                Link = openSearchDto.link,
                Text = openSearchDto.text,
                LikedByCurrentUser = openSearchDto.likedbycurrentuser,
                SeenByCurrentUser = openSearchDto.seenbycurrentuser,
                HighLightId = openSearchDto.highlightid,
                PostId = openSearchDto.postid,
                ArticleId = openSearchDto.articleid,
                Duration = openSearchDto.duration,
                IsLiked = openSearchDto.isliked,
                LikeCount = openSearchDto.likecount,
                StorySeensCount = openSearchDto.storyseenscount,
                CommentCount = openSearchDto.commentcount,
                DeletedBy = openSearchDto.deletedby
            };
        }
        public ListResponseBase<StoryDto> GetStories(bool myStories, User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            setMapsterConfig(currentUser);

            if (myStories)
            {
                //var res = repository
                //    // .Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
                //    .Where(d => d.DeletedBy == DeletedBy.NotDeleted)
                //    .Where(c => c.UserId == currentUser.Id)
                //    .ProjectToType<StoryDto>();
                //var res = repository
                //   // .Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
                //    .Where(d => d.DeletedBy == DeletedBy.NotDeleted)
                //    .Where(c => c.UserId == currentUser.Id)
                //    .ProjectToType<StoryDto>();
                var res = repository.GetMyStories(currentUser.Id, HoursGetStoryUser);

                return new ListResponseBase<StoryDto>(res);
            }
            var thresholdDate = DateTime.Now.AddHours(-36);
            var stories = repository.GetUserFollower().Where(c => c.FollowerId == currentUser.Id && c.Follower.IsDeletedAccount == false)
                .SelectMany(c => c.Followed.Stories)
                .Where(s => s.CreatedDate >= thresholdDate);
            //var stories = repository.Where<UserFollower>(c => c.FollowerId == currentUser.Id && c.Follower.IsDeletedAccount == false)
            //    .SelectMany(c => c.Followed.Stories);
            //var stories = repository.GetStories(currentUser.Id);
            var result = stories.ProjectToType<StoryDto>();

            return new ListResponseBase<StoryDto>(result);
        }
        public ListResponseBase<StoryDto> GetStoriesForAdmin(User currentUser)
        {
            //var currentUser = GetCurrentUser();
            //if (currentUser == null)
            //    return ResponseStatus.AuthenticationFailed;
            setMapsterConfig(currentUser);

            var result = repository.GetStoryDbSet()
                //.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
                .ProjectToType<StoryDto>();

            // var result = repository.GetStoriesForAdmin();
            //.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            //.ProjectToType<StoryDto>();

            return new ListResponseBase<StoryDto>(result);
        }
        private void setMapsterConfig(User currentUser)
        {

            if (currentUser == null)
                return;
            TypeAdapterConfig<Story, StoryDto>
                            .NewConfig()
                            .Map(dest => dest.IsLiked, src => src.StoryLikes.Any(x => x.UserId == currentUser.Id))
                            .Map(dest => dest.LikeCount, src => src.StoryLikes.Count)
                            .Map(dest => dest.StorySeensCount, src => src.StorySeens.Count)
                            .Map(dest => dest.CommentCount, src => src.StoryComments.Count);
        }
        public int HoursGetStoryUser
        {
            get
            {
                string hoursGetStoryUser = _configuration["HoursGetStoryUser"];
                if (int.TryParse(hoursGetStoryUser, out int parsedValue))
                {
                    return parsedValue;
                }
                else
                {
                    throw new InvalidOperationException("Invalid integer value in configuration.");
                }
            }
        }
    }
}
