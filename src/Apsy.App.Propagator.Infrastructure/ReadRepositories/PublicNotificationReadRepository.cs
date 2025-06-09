namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class PublicNotificationReadRepository
 : Repository<PublicNotification,DataWriteContext>, IPublicNotificationReadRepository
{
public PublicNotificationReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

	#endregion
	#region functions
	public IQueryable<PublicNotification> GetPublicNotification(int id)
	{
		var publicnotification = context.PublicNotification.Where(d => d.Id == id)
				.AsNoTracking().AsQueryable();
		return publicnotification;
	}
    public async Task<PublicNotification> GetFirstPublicNotification(int id)
    {
        var publicnotification = await context.PublicNotification
                .AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        return publicnotification;
    }
    public IQueryable<PublicNotification> GetPublicNotifications()
	{
		var publicnotification = context.PublicNotification
				.AsNoTracking().AsQueryable();
		return publicnotification;
	}
	#endregion
}
