
namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface INotificationService : IServiceBase<Notification, NotificationInput>
{

    Task SendFirebaseCloudMessage(Notification notification);
    Task<ListResponseBase<Notification>> SendNotificationToAllUsers(NotificationInput input, User currentUser);
    ResponseBase<Notification> ReadNotification(int id, User currentUser);
    ResponseBase<bool> ReadNotifications(int[] notificationIds, User currentUser);
    Task<ResponseBase<bool>> ReadNotificationCurrent(User currentUser);

    Task<ResponseBase<Notification>> SendNotificationToUser(int userId, NotificationInput input);
    
}