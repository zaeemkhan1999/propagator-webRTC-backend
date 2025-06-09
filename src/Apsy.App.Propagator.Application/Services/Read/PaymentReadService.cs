using Apsy.App.Propagator.Application.Services.ReadContracts;
using Propagator.Common.Services;
using Stripe;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class PaymentReadService : StripeSerivceBase<Payment, StripPaymentInput, User, CustomerInput, StripFullPaymentInput>, IPaymentReadService
    {
        private readonly IPaymentReadRepository _paymentRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUserReadRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IPostReadRepository _postRepository;
        private readonly IAdReadRepository _adRepository;
        private readonly IArticleReadRepository _articleRepository;
        private readonly ISettingsReadRepository _settingsRepository;
        public PaymentReadService(IPaymentReadRepository paymentRepository,
                 IConfiguration configuration,
                 IHttpContextAccessor httpContextAccessor,
                 ISubscriptionPlanRepository subscriptionPlanRepository,
                 IUserReadRepository userRepository,
                 IWebHostEnvironment env,
                 IPostReadRepository postRepository,
                 IAdReadRepository adRepository,
                 IArticleReadRepository articleRepository,
                 ISettingsReadRepository settingsRepository) : base(paymentRepository, httpContextAccessor)
        {
            _paymentRepository = paymentRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _userRepository = userRepository;
            _env = env;
            _postRepository = postRepository;
            _adRepository = adRepository;
            _articleRepository = articleRepository;
            _settingsRepository = settingsRepository;
        }
        public ResponseBase<string> GetPublishableKey() => _configuration.GetSection("Stripe:StripeConfigPK").Value;
        public ResponseBase<ViewPriceDto> GetViewPrice()
        {
            var numberOfPeoplePerUnit = Convert.ToInt32(_configuration.GetSection("NumberOfPeoplePerUnit").Value);
            var initialPrice = Convert.ToInt32(_configuration.GetSection("InitialPrice").Value);

            return new ViewPriceDto()
            {
                InitialPrice = initialPrice,
                NumberOfPeoplePerUnit = numberOfPeoplePerUnit
            };
        }
        public ListResponseBase<StripeAccountRequirementsError> GetAccountRequirements(User currentUser)
        {
            if (string.IsNullOrEmpty(currentUser.StripeAccountId))
                return ListResponseBase<StripeAccountRequirementsError>.Failure(ResponseStatus.NotFound);

            var accountService = new AccountService();
            var account = accountService.Get(currentUser.StripeAccountId);
            if (account == null)
                return ListResponseBase<StripeAccountRequirementsError>.Failure(ResponseStatus.NotFound);

            var errors = account?.Requirements?.Errors.Select(c => new StripeAccountRequirementsError
            {
                Code = c.Code,
                Reason = c.Reason,
                Requirement = c.Requirement
            }).AsQueryable();

            return ListResponseBase<StripeAccountRequirementsError>.Success(errors);
        }
        public int GetSubscriptionPlanIdByPriceId(string priceId)
        {
            var plan = _subscriptionPlanRepository.GetSubscriptionPlanByPriceId(priceId);
            if (plan == null)
            {
                return 0;
            }
            return plan.Id;
        }
    }
}
