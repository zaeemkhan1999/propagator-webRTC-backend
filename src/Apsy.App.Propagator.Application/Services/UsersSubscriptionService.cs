namespace Apsy.App.Propagator.Application.Services;

public class UsersSubscriptionService : ServiceBase<UsersSubscription, UsersSubscriptionInput>, IUsersSubscriptionService
{
    public UsersSubscriptionService(IUsersSubscriptionRepository repository,
        IHttpContextAccessor httpContextAccessor,
        ISubscriptionPlanRepository subscriptionPlanRepository,
        IUserRepository userRepository,
        IPaymentRepository paymentRepository) : base(repository)
    {
        _usersSubscriptionRepository = repository;
        _httpContextAccessor = httpContextAccessor;
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
    }

    private readonly IUsersSubscriptionRepository _usersSubscriptionRepository;
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public async Task<ResponseBase> ChargeUserSubscriptionPlanAsync(int userId,
        int subscriptionPlanId)
    {
        var user = await _usersSubscriptionRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return ResponseBase.Failure(ResponseStatus.NotFound);
        }

        var plan = await _usersSubscriptionRepository.GetSubscriptionPlanById(subscriptionPlanId);
        if (plan == null)
        {
            return ResponseBase.Failure(ResponseStatus.NotFound);
        }

        var prevUserSubscriptions = await _usersSubscriptionRepository
            .Where(x => x.UserId == user.Id &&
                        x.ExpirationDate >= DateTime.UtcNow &&
                        x.SubscriptionPlanId == subscriptionPlanId &&
                        x.Status != UserSubscriptionStatuses.Canceled)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();

        var expirationDate = DateTime.UtcNow;
        if (prevUserSubscriptions.Any())
        {
            expirationDate = prevUserSubscriptions.First().ExpirationDate;
         
            foreach (var userSubscription in prevUserSubscriptions)
            {
                userSubscription.Status = UserSubscriptionStatuses.Canceled;
            }
        }

        expirationDate = expirationDate.AddDays(plan.DurationDays);

        try
        {
            await _paymentRepository.BeginTransactionAsync();

            var payment = new Payment()
            {
                Amount = plan.Price,
                PaymentStatus = PaymentStatus.PayForSubscription,
                AmountWithoutDiscount = plan.Price,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                UsersSubscriptionId = subscriptionPlanId,
                PaymentConfirmationStatus = PaymentConfirmationStatus.Successful,
            };

            await _paymentRepository.AddAsync(payment);

            var userSubscription = new UsersSubscription()
            {
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
                SubscriptionPlanId = subscriptionPlanId,
                ExpirationDate = expirationDate,
                IsDeleted = false,
                LastModifiedDate = null,
                PaymentId = payment.Id,
                Status = UserSubscriptionStatuses.Active
            };

            await _paymentRepository.UpdateRangeAsync(prevUserSubscriptions);
            await _paymentRepository.AddAsync(userSubscription);

            payment.UsersSubscriptionId = userSubscription.Id;

            await _paymentRepository.UpdateAsync(payment);

            _paymentRepository.CommitTransaction();

            return ResponseBase.Success();
        }
        catch
        {
            _paymentRepository.RollBackTransaction();
            return ResponseBase.Failure(ResponseStatus.Failed);
        }
    }
}