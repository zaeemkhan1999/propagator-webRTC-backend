namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IPublicNotificationReadRepository : IRepository<PublicNotification>
{
	IQueryable<PublicNotification> GetPublicNotification(int id);
	Task<PublicNotification> GetFirstPublicNotification(int id);
	IQueryable<PublicNotification> GetPublicNotifications();
}
