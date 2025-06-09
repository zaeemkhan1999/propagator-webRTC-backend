using Apsy.App.Propagator.Application.DessignPattern.Posts;
using Apsy.App.Propagator.Application.GraphQL.Extensions;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common;
using Apsy.App.Propagator.Infrastructure.Extensions;
using Apsy.App.Propagator.Infrastructure.Redis;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using User = Apsy.App.Propagator.Domain.Entities.User;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class PostReadService : ServiceBase<Post, PostInput>, IPostReadService
    {
        private readonly IRedisCacheService _redisCache;
        private readonly IPostReadRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IPublisher _publisher;
        private readonly IEventStoreReadRepository _eventStoreRepository;
        private List<BaseEvent> _events;
        private readonly IApplicationLogReadRepository _logs;
        private readonly IAdReadRepository _adRepository;
        private readonly IInterestedUserService _interestedUserService;
        private readonly ITagReadRepository _tagRepository;
        private readonly ISettingsReadRepository _settingRepository;
        private readonly IPostLikeReadRepository _postLikeRepository;
        private readonly IUserReadRepository _userRepository;
        private readonly IViewPostReadRepository _viewPostRepository;
        private readonly ICommentReadRepository _commentRepository;
        private readonly IPlaceReadRepository _placeRepository;
        private readonly IDiscountReadRepository _discountRepository;
        private readonly IUserDiscountReadRepository _userDiscountRepository;
        private readonly ILinkReadRepository _linkRepository;
        private readonly CompressionApiClient _compressionApi;
        private readonly IUsersSubscriptionReadService _usersSubscriptionService;
        private readonly IFileUploadService _fileUploadService;
        public PostReadService(IPostReadRepository repository,
        IFileUploadService fileUploadService,
        IRedisCacheService redisCache,
        IHttpContextAccessor httpContextAccessor,
        IPaymentService paymentService,
        IConfiguration configuration,
        IPublisher publisher,
        CompressionApiClient compressionApiClient,
        IEventStoreReadRepository eventStoreRepository,
        IAdReadRepository adRepository,
        IApplicationLogReadRepository logs,
        IInterestedUserService interestedUserService,
        System.Net.Http.IHttpClientFactory httpClientFactory,
        ITagReadRepository tagRepository,
        ISettingsReadRepository settingRepository,
        IPostLikeReadRepository postLikeRepository,
        IViewPostReadRepository viewPostRepository,
        ICommentReadRepository commentRepository,
        IPlaceReadRepository placeRepository,
        IDiscountReadRepository discountRepository,
        ILinkReadRepository linkRepository,
        IUserDiscountReadRepository userDiscountRepository,
        IUsersSubscriptionReadService usersSubscriptionService,
        IUserReadRepository userRepository)
        : base(repository)
        {
            _redisCache = redisCache;
            _repository = repository;
            _fileUploadService = fileUploadService;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
            _configuration = configuration;
            _publisher = publisher;
            _eventStoreRepository = eventStoreRepository;
            _events = new List<BaseEvent>();
            _adRepository = adRepository;
            _interestedUserService = interestedUserService;
            _tagRepository = tagRepository;
            _settingRepository = settingRepository;
            _postLikeRepository = postLikeRepository;
            _viewPostRepository = viewPostRepository;
            _commentRepository = commentRepository;
            _placeRepository = placeRepository;
            _discountRepository = discountRepository;
            _userDiscountRepository = userDiscountRepository;
            _userRepository = userRepository;
            _linkRepository = linkRepository;
            _compressionApi = compressionApiClient;
            _logs = logs;
            _usersSubscriptionService = usersSubscriptionService;
        }
        private static readonly Regex videoExtensions = new Regex(@"\.(mp4|webm|ogg|mkv|mov|avi|flv|wmv|m4v|m3u8)$", RegexOptions.IgnoreCase);
        private static readonly Regex imageExtensions = new Regex(@"\.(jpg|jpeg|png|webp|gif|bmp|tiff|svg|ico)$", RegexOptions.IgnoreCase);
        ResponseBase<List<PostWatchHistory>> IPostReadService.GetAllWatchedHistory()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                throw new Exception("User not found");
            var post = _repository.GetWatchedHistory(currentUser.Id);
            return post;
        }
        SingleResponseBase<PostDto> IPostReadService.GetPost(int id)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var postQueryable = _repository.GetPostByUser(id, currentUser.Id);


            if (postQueryable.Any())
            {
                return new SingleResponseBase<PostDto>(postQueryable);
            }

            return ResponseStatus.NotFound;
        }
        CustomListResponseBase<PostDto> IPostReadService.GetPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
                var posts = _repository.GetPosts(currentUser);


            return new CustomListResponseBase<PostDto>(posts);
        }
        ListResponseBase<PostDto> IPostReadService.GetAdsPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            if (!currentUser.ProfessionalAccount)
                return new ListResponseBase<PostDto>(Array.Empty<PostDto>().AsQueryable());
            var posts = _repository.GetAdsPosts(currentUser);

            return new ListResponseBase<PostDto>(posts);
        }
        ListResponseBase<PostDto> IPostReadService.GetPromotedPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            if (!currentUser.ProfessionalAccount)
                return new ListResponseBase<PostDto>(Array.Empty<PostDto>().AsQueryable());
            var posts = _repository.GetPromotedPosts(currentUser);

            return new ListResponseBase<PostDto>(posts);
        }
        ListResponseBase<PostDto> IPostReadService.GetRandomPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var posts = _repository.GetRandomPosts(currentUser);

            return new ListResponseBase<PostDto>(posts);
        }
        async Task<PostExploreDto> IPostReadService.GetExploreRecommendedPostsAsync(int? lastId, int pageSize, string searchTerm, int skip, int take)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return ResponseStatus.AuthenticationFailed;
            }

            var posts = new List<PostDto>();
            List<PostDto> recommendedPosts;
            var postRecommended = new PostRecommendedHandler(this, _repository, _usersSubscriptionService, _httpContextAccessor);
            var recommend = postRecommended.Handle(GetPostType.Recommended, currentUser);
            recommendedPosts = recommend.Result.Where(x => string.IsNullOrEmpty(searchTerm) || x.Post.YourMind.ToLower().Contains(searchTerm.ToLower()))
                .Where(x => x.Post.PostType != PostType.Ads && x.Post.IsCreatedInGroup == false).ToList();

            var combinedPostsCount = posts.Concat(recommendedPosts).ToList();
            var combinedPosts = posts.Concat(recommendedPosts).Skip(skip).Take(take).ToList();

            var totalCount = combinedPostsCount.Count;

            var filtervalue = new PostExploreDto(combinedPosts.AsQueryable(),
                totalCount,
                (await BaseQuery(null).OrderByDescending(x => x.CreatedDate).LastOrDefaultAsync())?.Id != posts.LastOrDefault()?.Post.Id,
                lastId.HasValue);
            return filtervalue;

            IQueryable<Post> BaseQuery(int? id)
            {
                return _repository.GetExplorePostsAsyncBaseQuery(id, currentUser, searchTerm);
            }
        }
        async Task<PostExploreDto> IPostReadService.GetExploreRecommendedImagePostsAsync(int? lastId, int pageSize, string searchTerm, int skip, int take)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return ResponseStatus.AuthenticationFailed;
            }
            var posts = new List<PostDto>();
            List<PostDto> recommendedPosts;
            var postRecommended = new PostRecommendedHandler(this, _repository, _usersSubscriptionService, _httpContextAccessor);
            var recommend = postRecommended.Handle(GetPostType.Recommended, currentUser);
            recommendedPosts = recommend.Result.Where(x => string.IsNullOrEmpty(searchTerm) || x.Post.YourMind.ToLower().Contains(searchTerm.ToLower()))
                .Where(x => x.Post.PostType != PostType.Ads && x.Post.IsCreatedInGroup == false).ToList()
                .Where(x => IsImagePostItem(x.Post.PostItemsString)).ToList();

            var combinedPostsCount = posts.Concat(recommendedPosts).ToList();
            var combinedPosts = posts.Concat(recommendedPosts).Skip(skip).Take(take).ToList();

            var totalCount = combinedPostsCount.Count;

            var filtervalue = new PostExploreDto(combinedPosts.AsQueryable(),
                totalCount,
                (BaseImageQuery(null).OrderByDescending(x => x.CreatedDate).LastOrDefault())?.Id != posts.LastOrDefault()?.Post.Id,
                lastId.HasValue);
            return await Task.FromResult(filtervalue);

            IQueryable<Post> BaseImageQuery(int? id)
            {
                return _repository.GetExplorePostsAsyncBaseQuery(id, currentUser, searchTerm).ToList()
                    .Where(p => IsImagePostItem(p.PostItemsString)).AsQueryable();
            }
        }
        async Task<PostExploreDto> IPostReadService.GetExploreRecommendedVideoPostsAsync(int? lastId, int pageSize, string searchTerm, int skip, int take)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return ResponseStatus.AuthenticationFailed;
            }
            var posts = new List<PostDto>();
            List<PostDto> recommendedPosts;
            var postRecommended = new PostRecommendedHandler(this, _repository, _usersSubscriptionService, _httpContextAccessor);
            var recommend = postRecommended.Handle(GetPostType.Recommended, currentUser);
            recommendedPosts = recommend.Result.Where(x => string.IsNullOrEmpty(searchTerm) || x.Post.YourMind.ToLower().Contains(searchTerm.ToLower()))
                .Where(x => x.Post.PostType != PostType.Ads && x.Post.IsCreatedInGroup == false).ToList()
                .Where(x => IsVideoPostItem(x.Post.PostItemsString)).ToList();

            var combinedPostsCount = posts.Concat(recommendedPosts).ToList();
            var combinedPosts = posts.Concat(recommendedPosts).Skip(skip).Take(take).ToList();

            var totalCount = combinedPostsCount.Count;

            var filtervalue = new PostExploreDto(combinedPosts.AsQueryable(),
                totalCount,
                (BaseVideoQuery(null).OrderByDescending(x => x.CreatedDate).LastOrDefault())?.Id != posts.LastOrDefault()?.Post.Id,
                lastId.HasValue);
            return await Task.FromResult(filtervalue);

            IQueryable<Post> BaseVideoQuery(int? id)
            {
                return _repository.GetExplorePostsAsyncBaseQuery(id, currentUser, searchTerm).AsEnumerable()
                    .Where(p => IsVideoPostItem(p.PostItemsString)).AsQueryable();
            }
        }
        async Task<PostExploreDto> IPostReadService.GetExplorePostsAsync(int? lastId, int pageSize, string searchTerm)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return ResponseStatus.AuthenticationFailed;
            }

            var currentLastId = lastId;
            var currentPageSize = pageSize;
            var videoCount = (currentPageSize / 10) * 2;
            var posts = new List<PostDto>();
            while (true)
            {
                var result = await ProjectionQuery(currentLastId)
                    .Take(pageSize)
                    .ToListAsync();

                posts.AddRange(result);
                if (!result.Any() ||
                    result.Count < pageSize ||
                    posts.Count(x =>
                    {
                        var items = JsonConvert.DeserializeObject<List<PostItem>>(x.PostItemsString).ToList();
                        return items.Any(y => y.PostItemType == PostItemType.Video);
                    }) >= videoCount)
                {
                    break;
                }
                currentLastId = result.Last().Post.Id;
                currentPageSize += pageSize;
                videoCount = (currentPageSize / 10) * 2;
            }
            List<PostDto> recommendedPosts;
            try
            {
                recommendedPosts = await new EnhancedVideoRecommendationService().RecommendPostsForUser(_repository, currentUser);
            }
            catch
            {
                return ResponseStatus.AuthenticationFailed;
            }

            var combinedPosts = posts.Concat(recommendedPosts).ToList();

            var totalCount = combinedPosts.Count;

            return new PostExploreDto(combinedPosts.AsQueryable(),
                await BaseQuery(null).CountAsync(),
                (await BaseQuery(null).OrderByDescending(x => x.CreatedDate).LastOrDefaultAsync())?.Id != posts.LastOrDefault()?.Post.Id,
                lastId.HasValue);

            IQueryable<Post> BaseQuery(int? id)
            {
                return _repository.GetExplorePostsAsyncBaseQuery(id, currentUser, searchTerm);
            }

            IQueryable<PostDto> ProjectionQuery(int? id)
            {
                var baseQuery = BaseQuery(id).OrderByDescending(x => x.CreatedDate);
                var query = baseQuery
                 .Select(x => new PostDto
                 {
                     Post = x,
                     PostItemsString = x.PostItemsString,
                     IsLiked = x.Likes.Any(c => c.UserId == currentUser.Id && c.Liked),
                     IsViewed = x.UserViewPosts.Any(c => c.UserId == currentUser.Id),
                     IsNotInterested = x.NotInterestedPosts.Any(c => c.UserId == currentUser.Id),
                     IsSaved = x.SavePosts.Any(c => c.UserId == currentUser.Id),
                     IsYourPost = x.PosterId == currentUser.Id,
                     CommentCount = x.CommentsCount,
                     ShareCount = x.Messages.Count,
                     LikeCount = x.LikesCount,
                     ViewCount = x.Hits,
                     NotInterestedPostsCount = x.NotInterestedPostsCount,
                     PostComments = x.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                         .Select(c => new CommentDto
                         {
                             Comment = c,
                             IsLiked = c.LikeComments.Any(c => c.UserId == currentUser.Id),
                             HasChild = c.Children.Any(),
                             ChildrenCount = c.Children.Count,
                             LikeCount = c.LikeComments.Count
                         })
                         .ToList()
                 });
                return query;
            }
        }
        ResponseBase<bool> IPostReadService.CheckIsNewPostAvailable(DateTime from)
        {
            bool isAvailable = false;
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            isAvailable = _repository.CheckIsNewPostAvailable(from);

            return new(isAvailable);
        }
        ListResponseBase<PostDto> IPostReadService.GetTopPosts(DateTime from)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var postsQueryable = _repository.GetTopPosts(currentUser.Id, from);
            return new(postsQueryable);
        }
        ListResponseBase<PostDto> IPostReadService.GetFollowersPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var posts = _repository.GetFollowersPosts(currentUser.Id);
            return new(posts);
        }
        ListResponseBase<Comment> IPostReadService.GetComments(int postId)
        {
            var result = _commentRepository.GetCommentByPostId(postId);
            return ListResponseBase<Comment>.Success(result);
        }
        ListResponseBase<UserViewPost> IPostReadService.GetViews()
        {
            var views = _viewPostRepository.GetPostViews();
            return ListResponseBase<UserViewPost>.Success(views);
        }
        ListResponseBase<PostLikes> IPostReadService.GetLikedPosts()
        {
            var user = GetCurrentUser();
            if (user == null)
                return ListResponseBase<PostLikes>.Failure(ResponseStatus.NotFound);

            var likedPosts = _repository.GetPostLikesByUser(user.Id);
            return ListResponseBase<PostLikes>.Success(likedPosts);
        }
        ListResponseBase<SavePostDto> IPostReadService.GetSavedPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var posts = _repository.GetSavedPost(currentUser.Id);
            return new(posts);
        }
        private User GetCurrentUser(string token = null)
        {
            var User = _httpContextAccessor.HttpContext.User;
            if (!User.Identity.IsAuthenticated)
                return null;
            var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
            var user = JsonConvert.DeserializeObject<User>(userString);
            return user;
        }
        IQueryable<PostDto> IPostReadService.GetRecommendedPostsForHandler()
        {
            var currentUser = GetCurrentUser();
            var response = new EnhancedVideoRecommendationService().RecommendPostsForUserForHanlder(_repository, currentUser);

            return response.AsQueryable();
        }
        async Task<ListResponseBase<PostDto>> IPostReadService.GetRecommendedPosts()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var response = await new EnhancedVideoRecommendationService().RecommendPostsForUser(_repository, currentUser);

            return new ListResponseBase<PostDto>(response.AsQueryable());
        }
        public static bool IsVideoPostItem(string postItem)
        {

            if (string.IsNullOrEmpty(postItem)) return false;

            try
            {
                var jsonArray = JArray.Parse(postItem);
                foreach (var item in jsonArray)
                {
                    var content = (string)item["Content"];
                    if (!string.IsNullOrEmpty(content) && videoExtensions.IsMatch(content))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsImagePostItem(string postItem)
        {

            if (string.IsNullOrEmpty(postItem)) return false;

            try
            {
                var jsonArray = JArray.Parse(postItem);
                foreach (var item in jsonArray)
                {
                    var content = (string)item["Content"];
                    if (!string.IsNullOrEmpty(content) && imageExtensions.IsMatch(content))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
