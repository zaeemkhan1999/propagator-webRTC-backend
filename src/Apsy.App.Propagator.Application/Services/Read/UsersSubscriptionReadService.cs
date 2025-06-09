using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class UsersSubscriptionReadService :ServiceBase<UsersSubscription, UsersSubscriptionInput>, IUsersSubscriptionReadService
    {
        private readonly IUsersSubscriptionReadRepository _usersSubscriptionRepository;
        private readonly ISubscriptionPlanReadRepository _subscriptionPlanRepository;
        private readonly IUserReadRepository _userRepository;
        private readonly IPaymentReadRepository _paymentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersSubscriptionReadService(IUsersSubscriptionReadRepository repository,
       IHttpContextAccessor httpContextAccessor,
       ISubscriptionPlanReadRepository subscriptionPlanRepository,
       IUserReadRepository userRepository,
       IPaymentReadRepository paymentRepository) : base(repository)
        {
            _usersSubscriptionRepository = repository;
            _httpContextAccessor = httpContextAccessor;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }
        public async Task<ResponseBase<UsersSubscriptionsFeaturesDto>> GetUsersSubscriptionsFeatures()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var usersSubscriptionsFeatures = new UsersSubscriptionsFeaturesDto();

            var usersSubscription = await _usersSubscriptionRepository.GetUsersSubscriptions()
                .Where(x => x.UserId == currentUser.Id &&
                        x.ExpirationDate >= DateTime.UtcNow &&
                        x.Status == UserSubscriptionStatuses.Active)
                .Include(x => x.SubscriptionPlan)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (usersSubscription == null)
                return usersSubscriptionsFeatures;

            usersSubscriptionsFeatures.ExpirationDate = usersSubscription.ExpirationDate;
            usersSubscriptionsFeatures.SubscriptionPlanId = usersSubscription.SubscriptionPlanId;
            usersSubscriptionsFeatures.Supportbadge = usersSubscription.SubscriptionPlan.Supportbadge;
            usersSubscriptionsFeatures.RemoveAds = usersSubscription.SubscriptionPlan.RemoveAds;
            usersSubscriptionsFeatures.AllowDownloadPost = usersSubscription.SubscriptionPlan.AllowDownloadPost;
            usersSubscriptionsFeatures.AddedToCouncilGroup = usersSubscription.SubscriptionPlan.AddedToCouncilGroup;

            return usersSubscriptionsFeatures;
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
