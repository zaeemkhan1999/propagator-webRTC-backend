namespace Apsy.App.Propagator.Domain.Entities;

public class NotInterestedPost : UserKindDef<User>
{
    public Post Post { get; set; }
    public int PostId { get; set; }
}
