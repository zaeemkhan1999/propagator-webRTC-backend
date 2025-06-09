using FirebaseAdmin.Messaging;
using System.Security.Policy;
using Twilio.TwiML.Voice;
using Notification = Apsy.App.Propagator.Domain.Entities.Notification;
using Task = System.Threading.Tasks.Task;

namespace Apsy.App.Propagator.Application.Services;

public class NotificationService : ServiceBase<Notification, NotificationInput>, INotificationService
{
    public NotificationService(
        INotificationRepository repository,
        FirebaseAppCreator firebaseApp,
        IPublisher publisher,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration) : base(repository)
    {
        this.repository = repository;
        _firebaseApp = firebaseApp;
        _publisher = publisher;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly INotificationRepository repository;
    private readonly FirebaseAppCreator _firebaseApp;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    private readonly IUserRepository _userRepository;

    public async Task SendFirebaseCloudMessage(Notification notification)
    {
        _firebaseApp.GetFirebaseApp();

        // The topic name can be optionally prefixed with "/topics/".
        var topic = "user" + notification.RecieverId;

        // See documentation on defining a message payload.
        var message = new FirebaseAdmin.Messaging.Message()
        {
            // Topic = topic,
            // Token = "fEM0wJxlvI4:APA91bHF_mDeJZYxXcof7IafdhNoldKbYlcpap6764t6L5I55bX0N9JecFede-cUrrczZvqANh-nQ7yMOil7UKELlprtbJPT_eUou9iDnKGXb70oNT2pTP8QomVSzL4a4ZyQvI08oFas",
            Topic = topic,

            Notification = new FirebaseAdmin.Messaging.Notification
            {

                Body = notification.Text,
                Title = notification.Text,
            },
            Data = new Dictionary<string, string>
            {
                { "notification", JsonConvert.SerializeObject(notification, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) }
            }


            // Android = new AndroidConfig()
            // {
            // },
        };

        try
        {
            // Send a message to the devices subscribed to the provided topic.
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // Response is a message ID string.
            Console.WriteLine("Successfully sent message: " + response);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ListResponseBase<Notification>> SendNotificationToAllUsers(NotificationInput input, User currentUser)
    {
        var users = (IQueryable<User>)_userRepository.GetUserId(currentUser.Id);
        if (input.Gender != null)
            users = users.Where(c => c.Gender == input.Gender);
        if (input.StartDate != null)
            users = users.Where(c => c.DateOfBirth >= input.StartDate);
        if (input.EndDate != null)
            users = users.Where(c => c.DateOfBirth <= input.EndDate);

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
                Text = input.Text,
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
            catch { }
        }
        return ResponseStatus.Success;
    }

    public override ResponseStatus SoftDelete(int entityId)
    {
        var notificationResult = repository.GetSoftDelete(entityId);

        if (notificationResult.Status != ResponseStatus.Success)
            return notificationResult;

        var User = _httpContextAccessor.HttpContext.User;
        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);

        if (notificationResult.Result.RecieverId != user.Id)
            return ResponseBase<Notification>.Failure(ResponseStatus.AuthenticationFailed);

        return base.SoftDelete(entityId);
    }

    public ResponseBase<Notification> ReadNotification(int id, User currentUser)
    {
        var notification = repository.GetNotification(id);
        if (notification == null)
            return ResponseBase<Notification>.Failure(ResponseStatus.NotFound);

        if (notification.RecieverId != currentUser.Id)
            return ResponseBase<Notification>.Failure(ResponseStatus.AuthenticationFailed);

        notification.IsReaded = true;
        return repository.Update(notification);
    }

    public ResponseBase<bool> ReadNotifications(int[] notificationIds, User currentUser)
    {
        var notificaitons = repository.GetNotifications(notificationIds);
        foreach (var item in notificaitons)
        {
            if (currentUser.Id != item.RecieverId)
                return ResponseStatus.NotAllowd;
            item.IsReaded = true;
        }
        repository.UpdateRange(notificaitons);
        return true;
    }

    public async Task<ResponseBase<bool>> ReadNotificationCurrent(User currentUser)
    {
        var notificaitons = repository.GetNotifications(currentUser.Id);
        foreach (var item in notificaitons)
        {
            item.IsReaded = true;
        }

        await repository.UpdateRangeAsync(notificaitons);

        return true;
    }

    public async Task<ResponseBase<Notification>> SendNotificationToUser(int userId, NotificationInput input)
    {
        var notification = new Notification
        {
            NotificationType = NotificationType.Welcome,
            IsReaded = false,
            RecieverId = userId,
            SenderId = 0,
            Text = input.Text
        };   
        repository.Add(notification);

        try
        {
            await SendFirebaseCloudMessage(notification);
        }
        catch (Exception ex)
        {
            
            Console.WriteLine("Firebase send failed: " + ex.Message);
        }
        return ResponseBase<Notification>.Success(notification);
    }

}