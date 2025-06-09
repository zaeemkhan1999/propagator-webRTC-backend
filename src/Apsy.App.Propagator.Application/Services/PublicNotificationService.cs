namespace Apsy.App.Propagator.Application.Services;

public class PublicNotificationService : ServiceBase<PublicNotification, PublicNotificationInput>, IPublicNotificationService
{
    public PublicNotificationService(
        IPublicNotificationRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IUserRepository userRepository) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        this.userRepository = userRepository;
    }
    private readonly IPublicNotificationRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    private readonly IUserRepository userRepository;
    public async Task<ResponseBase<PublicNotification>> AddPublicNotification(PublicNotificationInput input)
    {
        var publicNotification = input.Adapt<PublicNotification>();

        await repository.AddAsync(publicNotification);

        return ResponseBase<PublicNotification>.Success(publicNotification);
    }

    public async Task<ResponseBase<PublicNotification>> UpdatePublicNotification(PublicNotificationInput input)
    {
        //var query = repository.Where(d => d.Id == input.Id);
        var query = repository.GetPublicNotification((int)input.Id);

        if (input.Id == 0 || !query.Any())
            return ResponseStatus.NotFound;

        var publicNotification = await query.FirstOrDefaultAsync();
        input.Adapt(publicNotification);

        await repository.UpdateAsync(publicNotification);

        return ResponseBase<PublicNotification>.Success(publicNotification);
    }

    public async Task<ResponseBase> DeletePublicNotification(int id)
    {
        //var query = repository.Where(d => d.Id == id);
        var query = repository.GetPublicNotification(id);

        if (id == 0 || !query.Any())
            return ResponseStatus.NotFound;

        var publicNotification = await query.FirstOrDefaultAsync();

        await repository.RemoveAsync(publicNotification);

        return ResponseBase.Success();
    }

    public async Task<ResponseBase> Send(int notificationId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        //var notif = await repository.Where(d => d.Id == notificationId).FirstOrDefaultAsync();
        var notif = await repository.GetFirstPublicNotification(notificationId);
        List<User> users = null;

        if (notif.IsSendAll)
        {
            //users = await userRepository.Where(d => d.Gender == notif.Gender && (DateTime.Today.Year - d.DateOfBirth.Year) >= notif.FromAge && (DateTime.Today.Year - d.DateOfBirth.Year) <= notif.ToAge).ToListAsync();
            users = await userRepository.GetUsers(notif).ToListAsync();
        }
        else
        {
            //users = await userRepository.GetDbSet().ToListAsync();
            users = await userRepository.GetUsers().ToListAsync();
        }

        if (users.Count == 0 || notif is null)
            return ResponseStatus.NotFound;

        var notifications = new List<Notification>();
        var notificationEvents = new List<NotificationEventDto>();

        foreach (var item in users)
        {
            var notification = new Notification()
            {
                NotificationType = NotificationType.NotificaitonByAdmin,
                IsReaded = false,
                RecieverId = item.Id,
                SenderId = currentUser.Id,
                Text = notif.Text,
            };

            notificationEvents.Add(new NotificationEventDto
            {
                Notification = notification,
                Sender = currentUser,
                Reciever = item
            });
            notifications.Add(notification);
        }

        repository.AddRange(notifications);

        foreach (var item in notificationEvents)
        {
            try
            {
                await _publisher.Publish(new AdminSentEvent(item.Sender.Id, item.Reciever.Id, item.Notification));
            }
            catch
            {
            }
        }

        return ResponseStatus.Success;
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

