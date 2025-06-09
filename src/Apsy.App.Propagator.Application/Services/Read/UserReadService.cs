using Aps.CommonBack.Auth.Services;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Propagator.Common.Services.Contracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class UserReadService : UserServiceBase<User, UserInput>, IUserReadService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IUserReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private List<BaseEvent> _events;
        private readonly IEventStoreReadRepository _eventStoreRepository;
        private readonly IUserLoginReadRepository userLoginRepository;
        private readonly TwilioVerifySettingsVM _settings;
        private readonly IConfiguration _configuration;
        private readonly IJwtManagerService _jwtManagerService;
        //private readonly FirebaseAppCreator _firebaseApp;
        private readonly IWebHostEnvironment environment;
        private readonly string _confirmEmailUrl;
        private readonly string _resetpasswordUrl;
        private readonly IResetPasswordRequestReadRepository _resetPasswordRequestRepository;
        private readonly IUsersSubscriptionReadRepository _usersSubscriptionRepository;
        private readonly IPostReadRepository _postRepository;
        public UserReadService(
       UserManager<AppUser> userManager,
       IUserStore<AppUser> userStore,
       SignInManager<AppUser> signInManager,
       //IEmailSender emailSender,
       RoleManager<IdentityRole> roleManager,

       IUserReadRepository repository,
       IHttpContextAccessor httpContextAccessor,
       IEventStoreReadRepository eventStoreRepository,

       IOptions<TwilioVerifySettingsVM> settings,
       IConfiguration configuration,
       IWebHostEnvironment environment,
       IJwtManagerService jwtManagerService,
       IUserLoginReadRepository userLoginRepository,
       IPostReadRepository postRepository,
       IResetPasswordRequestReadRepository resetPasswordRequestRepository,
       IUsersSubscriptionReadRepository usersSubscriptionRepository) : base(repository)
        //FirebaseAppCreator firebaseApp) : base(repository)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _roleManager = roleManager;

            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _eventStoreRepository = eventStoreRepository;
            _events = new List<BaseEvent>();
            _postRepository = postRepository;
            _settings = settings?.Value;
            _configuration = configuration;
            this.environment = environment;
            _jwtManagerService = jwtManagerService;
            //_firebaseApp = firebaseApp;
            _confirmEmailUrl = _configuration["BaseUrl"] + _configuration["ConfirmEmailUrl"];
            _resetpasswordUrl = _configuration["BaseUrl"] + _configuration["ResetpasswordUrl"];
            this.userLoginRepository = userLoginRepository;
            _resetPasswordRequestRepository = resetPasswordRequestRepository;
            _usersSubscriptionRepository = usersSubscriptionRepository;
        }
        public async Task<ListResponseBase<UserClaimsViewModel>> GeteUserPermission(string username)
        {
            if (string.IsNullOrEmpty(username))
                return ResponseStatus.NotFound;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return ResponseStatus.NotFound;

            var result = (await _userManager.GetClaimsAsync(user)).Select(g => new UserClaimsViewModel { Type = g.Type, Value = g.Value }).AsQueryable();

            return new(result);
        }
        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }

        public ResponseBase<SingleUserDto> GetSingleUserDto(int userId)
        {
            var userFromDb = repository.GetUserByIdAsync(userId);
            if (userFromDb == null)
                return ResponseStatus.NotFound;

            SingleUserDto singleUserDto = new();

            var postCount = repository.GetPost().Where(c => c.PosterId == userId && !c.IsCreatedInGroup).Count();
            var articleCount = repository.GetArticles().Where(c => c.UserId == userId && !c.isCreatedInGroup).Count();

            singleUserDto.ContentCount = postCount + articleCount;
            singleUserDto.FollowerCount = repository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != userId) && c.FollowedId == userId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).Count();

            singleUserDto.FollwingCount = repository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != userId) && c.FollowerId == userId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).Count();

            return singleUserDto;
        }
        public bool UsernameExist(User currentUser, string userName)
        {
            if (currentUser is null)
                return repository.GetUser().Any(c => !string.IsNullOrEmpty(c.Username) && c.Username == userName);
            return repository.GetUser().Any(c => c.Id != currentUser.Id
                            && !string.IsNullOrEmpty(c.Username) && c.Username == userName);
        }
        public ResponseBase<CurrentUserDto> GetCurrentUserFromDb()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var user = Get(currentUser.Id);
            var twoFactorEnabled = repository.GetUser().Where(c => c.Id == currentUser.Id).Select(x => x.AppUser.TwoFactorEnabled).FirstOrDefault();

            var currentUsers = user.Result?.Adapt<CurrentUserDto>();
            currentUsers.FollowerCount = repository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != currentUser.Id) && c.FollowedId == currentUser.Id && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).Count();
            currentUsers.FollwingCount = repository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != currentUser.Id) && c.FollowerId == currentUser.Id && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).Count();
            currentUsers.EnableTwoFactorAuthentication = twoFactorEnabled;
            currentUsers.PostCount = _postRepository.GetPostCount(currentUser);
            return currentUsers;
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
        public ResponseBase<SingleUserDto> GetUserByIdDto(int userId, int currentUserId)
        {
            var user = repository.GetUserByIdAsync(userId);
            if (user is null)
                return ResponseStatus.NotFound;

            SingleUserDto singleUserDto = new();

            var postCount = repository.GetPost().Where(c => c.PosterId == userId && !c.IsCreatedInGroup).Count();
            var articleCount = repository.GetArticles().Where(c => c.UserId == userId && !c.isCreatedInGroup).Count();

            singleUserDto.ContentCount = postCount + articleCount;
            singleUserDto.FollowerCount = repository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != userId) && c.FollowedId == userId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).Count();

            singleUserDto.FollwingCount = repository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != userId) && c.FollowerId == userId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).Count();

            singleUserDto.IsFollowing = repository.GetUserFollowers()
            .Any(c => c.FollowerId == currentUserId && c.FollowedId == userId && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted);

            singleUserDto.user = user;

            return singleUserDto;
        }

        public ListResponseBase<UserSusciopiousDto> GetUserLogin()
        {
            var last = userLoginRepository.GetDbSet().GroupBy(d => d.AppUserId).Select(g => new UserSusciopiousDto
            {
                Id = g.OrderByDescending(d => d.CreatedDate).FirstOrDefault().Id,
                DisplayName = g.FirstOrDefault().AppUser.User.DisplayName,
                UserId = g.FirstOrDefault().AppUser.UserId,
                SigInIp = g.OrderByDescending(d => d.CreatedDate).FirstOrDefault().SigInIp,
                SignOutIp = g.OrderByDescending(d => d.CreatedDate).FirstOrDefault().SignOutIp,
                ImageAddress = g.FirstOrDefault().AppUser.User.ImageAddress,
                Cover = g.FirstOrDefault().AppUser.User.Cover,
                UserName = g.FirstOrDefault().AppUser.User.Username,
                SigInTime = g.OrderByDescending(d => d.CreatedDate).FirstOrDefault().SigInTime,
                SigOutTime = g.OrderByDescending(d => d.CreatedDate).FirstOrDefault().SigOutTime
            }
            ).OrderByDescending(d => d.Id).AsQueryable();

            return ListResponseBase.Success(last);
        }
        public ResponseBase<bool> IsTwoFacorEnabled()
        {
            var currentUser = GetCurrentUser();
            return repository.GetUser().Where(c => c.Id == currentUser.Id).Select(x => x.AppUser.TwoFactorEnabled).FirstOrDefault();
        }
        public ResponseBase<AdminInfoDto> GetAdminInfo()
        {
            var admin = repository.GetUser().Where(c => c.UserTypes == UserTypes.Admin).FirstOrDefault();
            if (admin is null)
                return ResponseStatus.NotFound;

            AdminInfoDto adminInfoDto = admin.Adapt<AdminInfoDto>();
            return adminInfoDto;
        }
        public ResponseBase<User> GetUserById(int userId)
        {
            var user = repository.GetUserByIdAsync(userId);
            if (user is null)
                return ResponseStatus.NotFound;


            return user;
        }
        public async Task<ListResponseBase<User>> GetListUsersHaveNotBlocked()
        {
            var currentUser = GetCurrentUser();

            var query = await Task.Run(() => repository.GetUser().Where(d => !d.Blocks.Any(t => t.BlockedId == currentUser.Id)));

            return ListResponseBase<User>.Success(query);
        }
        public ListResponseBase<ResetPasswordRequest> GetResetPasswordRequests()
        {
            return new ListResponseBase<ResetPasswordRequest>(repository.GetResetPasswordRequest());
        }

        public string GetCurrentUsersPhoneNumber()
        {
            var currentEmail = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return currentEmail;
        }

        public string GetCurrentUsersUsername()
        {
            var currentEmail = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            return currentEmail;
        }

        public async Task<ResponseBase<AppUser>> GetAppUser(ClaimsPrincipal claims)
        {
            if (claims.Claims == null || !claims.Claims.Any())
            {
                return ResponseBase<AppUser>.Failure(ResponseStatus.AuthenticationFailed);
            }
            var email = GetCurrentUsersEmail();

            return await _userManager.FindByEmailAsync(email);
        }
        public string GetCurrentUsersEmail()
        {
            var currentEmail = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            return "taha@propagator.ca";
        }
    }
}
