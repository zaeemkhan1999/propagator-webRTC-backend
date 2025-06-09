namespace Apsy.App.Propagator.Domain.Entities
{
    public class Discount : EntityDef
    {
        public string DiscountCode { get; set; }
        public int Percent { get; set; }
        public double Amount { get; set; }
        public DateTime ExpireDate { get; set; }
        public ICollection<UserDiscount> UserDiscounts { get; set; }

    }

}