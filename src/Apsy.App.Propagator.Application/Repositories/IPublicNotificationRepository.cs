namespace Apsy.App.Propagator.Application.Repositories;

public interface IPublicNotificationRepository : IRepository<PublicNotification>
{
	IQueryable<PublicNotification> GetPublicNotification(int id);
	Task<PublicNotification> GetFirstPublicNotification(int id);
	IQueryable<PublicNotification> GetPublicNotifications();
}
