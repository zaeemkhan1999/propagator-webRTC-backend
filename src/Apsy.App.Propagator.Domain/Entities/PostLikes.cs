namespace Apsy.App.Propagator.Domain.Entities;

public class PostLikes : EntityDef /*: PostLikeDef<User, Post>*/
{
    public int UserId { get; set; }

    public User User { get; set; }

    public int PostId { get; set; }

    public Post Post { get; set; }

    public bool Liked { get; set; }

    public List<Notification> Notifications { get; set; }

}