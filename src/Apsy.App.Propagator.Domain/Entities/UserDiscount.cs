namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserDiscount : EntityDef
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int DiscountId { get; set; }

        public Discount Discount { get; set; }

    }
}