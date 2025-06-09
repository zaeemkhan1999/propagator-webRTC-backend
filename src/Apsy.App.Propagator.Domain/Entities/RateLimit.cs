namespace Apsy.App.Propagator.Domain.Entities
{
    public class RateLimit
    {
        public int Id { get; set; }
        public string  LimitType { get; set; }
        public int LimitPrDay { get; set; }
        
    }
}
