namespace Apsy.App.Propagator.Domain.Entities;

public class ArticleLike : UserKindDef<User>
{
    public Article Article { get; set; }
    public int ArticleId { get; set; }
    public bool Liked { get; set; }

    public ICollection<Notification> Notifications { get; set; }
}