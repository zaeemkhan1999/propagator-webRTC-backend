using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class PostLikeReadService : ServiceBase<PostLikes, PostLikeInput>, IPostLikeReadService
    {
        public PostLikeReadService(IPostLikeRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly IPostLikeRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ListResponseBase<PostLikesDto> GetPostLikes()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            repository.GetDbSet().Load();
            //var posts = repository.GetDbSet();                
            var posts = repository.GetPostLikesByUser(currentUser.Id);
            return new(posts);
        }

        public ListResponseBase<PostLikesDto> GetPostLikesUsers(int postId)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var posts = repository.GetPostLikesUsers(postId);
            return new(posts);
        }

        private User GetCurrentUser()
        {
            var User = _httpContextAccessor.HttpContext.User;
            if (!User.Identity.IsAuthenticated)
                return null;

            var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
            var user = JsonConvert.DeserializeObject<User>(userString);
            return user;
        }
    }
}
