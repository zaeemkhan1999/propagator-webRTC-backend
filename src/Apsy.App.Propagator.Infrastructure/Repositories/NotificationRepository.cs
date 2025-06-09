namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class NotificationRepository : Repository<Notification, DataReadContext>, INotificationRepository
{
    public NotificationRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;

    public List<Notification> GetNotifications(int UserId)
    {
        return context.Notification.Where(x => x.RecieverId == UserId).ToList();
    }

    public List<Notification> GetNotifications(int[] notificationIds)
    {
        return context.Notification.Where(c => notificationIds.Contains(c.Id)).ToList();
    }

    public Notification GetNotification(int notificationId)
    {
        return context.Notification.Where(c => c.Id== notificationId).FirstOrDefault();
    }
    public SingleResponseBase<Notification> GetSoftDelete(int id, bool checkDeleted = false)
    {
        IQueryable<Notification> queryable = context.Notification.Where(x=>x.Id==id);
        if (queryable.Any())
        {
            return new SingleResponseBase<Notification>(queryable);
        }

        return ResponseStatus.NotFound;
    }

    public IQueryable<NotificationDto> GetNotifs(int userId)
    {

        // var Ist = context.Notification.Where(c => c.RecieverId == Id && c.Post.IsDeleted==false );
        //    var second=Ist.Select(c => new NotificationDto
        //    {
        //        Notification = c,
        //        IFollowUser = c.Reciever.Followees.FirstOrDefault(x => x.FollowerId == Id && x.FollowedId == c.SenderId &&x.IsDeleted==false),
        //    }).AsQueryable();
        // return second;

        return context.Notification
            .Include(n => n.Reciever) // Ensure Receiver is loaded
            .Include(n => n.Sender)   // Ensure Sender is loaded
            .Include(n => n.Post)     // Ensure Post is loaded (if applicable)
            .Include(n => n.Comment)  // Ensure Comment is loaded (if applicable)
            .Include(n => n.Article)
            .Include(n => n.PostLike)
            .Where(n => n.RecieverId == userId && !n.IsDeleted) // Filter by user and non-deleted notifications
            .Select(n => new NotificationDto
            {
                Notification = n,
                IFollowUser = n.Reciever.Followees
                    .FirstOrDefault(f => f.FollowerId == userId
                                    && f.FollowedId == n.SenderId
                                    && !f.IsDeleted),
                RelatedPost = n.Post,
                RelatedComment = n.Comment,
                RelatedArticle = n.Article,
                RelatedPostLikes = n.PostLike
            })
            .Where(dto =>
            (dto.RelatedPost == null || (!dto.RelatedPost.IsDeleted && dto.RelatedPost.DeletedBy == 0)) &&
            (dto.RelatedComment == null || (!dto.RelatedComment.IsDeleted &&
                                            !dto.RelatedComment.Post.IsDeleted &&
                                            dto.RelatedComment.DeletedBy == 0)) &&
            (dto.RelatedArticle == null || (!dto.RelatedArticle.IsDeleted && dto.RelatedArticle.DeletedBy == 0)) &&
            (dto.RelatedPostLikes == null || !dto.RelatedPostLikes.IsDeleted && dto.RelatedPostLikes.Post.DeletedBy==0 ))
        .AsQueryable();
    }
}