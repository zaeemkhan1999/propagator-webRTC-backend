namespace Apsy.App.Propagator.Domain.Entities;

public class UserSearchPost : UserKindDef<User>
{
    public Post Post { get; set; }
    public int PostId { get; set; }
}
