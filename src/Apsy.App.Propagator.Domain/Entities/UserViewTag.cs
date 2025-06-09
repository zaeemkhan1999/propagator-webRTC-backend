namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserViewTag : UserKindDef<User>
    {
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
