namespace Apsy.App.Propagator.Application.Services;
public class SuspendService : ServiceBase<Suspend, SuspendInput>, ISuspendService
{
    public SuspendService(
        ISuspendRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,
        IUserRepository userRepository,
        IPublisher publisher) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
        this.userRepository = userRepository;
        _publisher = publisher;
    }

    private readonly ISuspendRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IUserRepository userRepository;
    private List<BaseEvent> _events;
    private IPublisher _publisher;

    public override ResponseBase<Suspend> Add(SuspendInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var suspendedUser = repository.GetUserById(input.UserId);

        if (suspendedUser == null)
            return ResponseStatus.UserNotFound;

        if (suspendedUser.UserTypes == UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        if (suspendedUser.IsSuspended)
            return CustomResponseStatus.AccountAlreadySuspended;

        DateTime startDateTime = DateTime.Today; //Today at 00:00:00
        DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

        var adminTodayLimitation =
                repository
                .GetAdminTodayLimitation().Where(a => a.UserId == currentUser.Id && a.CreatedDate > startDateTime && a.CreatedDate < endDateTime)
                .FirstOrDefault();

        if (currentUser.UserTypes == UserTypes.Admin && adminTodayLimitation != null && adminTodayLimitation.SuspendedCount >= 20)
            return CustomResponseStatus.LimitTheNumberOfSuspend;

        if (adminTodayLimitation == null)
        {
            var newAdminTodayLimitation = new AdminTodayLimitation()
            {
                UserId = currentUser.Id,
                SuspendedCount = 1
            };
            repository.Add(newAdminTodayLimitation);
        }
        else if (adminTodayLimitation.SuspendedCount < 20)
        {
            adminTodayLimitation.SuspendedCount++;
            repository.Update(adminTodayLimitation);
        }

        var result = base.Add(input);
        result.Result.User = suspendedUser;

        suspendedUser.IsSuspended = true;
        suspendedUser.SuspensionLiftingDate = DateTime.UtcNow.AddDays(input.DayCount);
        userRepository.Update(suspendedUser);

        result.Result.RaiseEvent(ref _events, currentUser, CrudType.UsersSuspendedEvent);
        _eventStoreRepository.SaveEvents(_events);

        try
        {
            _publisher.Publish(new SupsendEvent( currentUser.Id, input.UserId)).GetAwaiter().GetResult();
        }
        catch
        {
        }
        return result;
    }

    public ResponseBase<User> UnSuspend(int userId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var suspendedUser = repository.GetUserById(userId);

        suspendedUser.IsSuspended = false;
        suspendedUser.SuspensionLiftingDate = null;
        userRepository.Update(suspendedUser);

        suspendedUser.RaiseEvent(ref _events, currentUser, false, CrudType.UsersUnSuspendedEvent);
        _eventStoreRepository.SaveEvents(_events);

        return repository.Update(suspendedUser);
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