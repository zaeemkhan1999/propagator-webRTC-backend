
namespace Apsy.App.Propagator.Domain.Entities;

public class StoryLike : UserKindDef<User>
{
    public Story Story { get; set; }
    public int StoryId { get; set; }
    public bool Liked { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}
