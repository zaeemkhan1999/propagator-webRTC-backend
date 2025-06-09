namespace Apsy.App.Propagator.Application.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    List<Notification> GetNotifications(int UserId);
    IQueryable<NotificationDto> GetNotifs(int UserId);
    List<Notification> GetNotifications(int[] notificationIds);
    Notification GetNotification(int notificationId);
    public SingleResponseBase<Notification> GetSoftDelete(int id, bool checkDeleted = false);
}
