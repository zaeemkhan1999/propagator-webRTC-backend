namespace Apsy.App.Propagator.Domain.Entities;

public class UserViewPost : UserKindDef<User>
{
    public Post Post { get; set; }
    public int PostId { get; set; }
}