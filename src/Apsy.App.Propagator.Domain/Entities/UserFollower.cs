
using System.ComponentModel.DataAnnotations.Schema;

namespace Apsy.App.Propagator.Domain.Entities;
public class UserFollower : EntityDef
{
    public bool HasSeenStory { get; set; }

    [ForeignKey("FollowerId")]
    public int FollowerId { get; set; }

    public User Follower { get; set; }

    [ForeignKey("FollowedId")]
    public int FollowedId { get; set; }

    public User Followed { get; set; }

    public bool IsMutual { get; set; }

    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;


    public FolloweAcceptStatus FolloweAcceptStatus { get; set; }
    public ICollection<Notification> Notifications { get; set; }

}