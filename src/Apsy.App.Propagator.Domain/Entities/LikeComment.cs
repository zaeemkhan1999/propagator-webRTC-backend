namespace Apsy.App.Propagator.Domain.Entities;

public class LikeComment : UserKindDef<User>
{
    //public Notification Notification { get; set; }
    //public int NotificationId { get; set; }
    public Comment Comment { get; set; }
    public int CommentId { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}