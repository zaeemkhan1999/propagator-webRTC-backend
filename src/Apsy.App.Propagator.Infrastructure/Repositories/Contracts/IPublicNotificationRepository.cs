namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IPublicNotificationRepository : IRepository<PublicNotification>
{
	IQueryable<PublicNotification> GetPublicNotification(int id);
	Task<PublicNotification> GetFirstPublicNotification(int id);
	IQueryable<PublicNotification> GetPublicNotifications();
}
