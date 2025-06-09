namespace Apsy.App.Propagator.Domain.Entities
{
    public class AppealAds : EntityDef
    {
        public Ads Ads { get; set; }
        public int AdsId { get; set; }
        public string Description { get; set; }
        public string ReasonReject { get; set; }
        public AppealStatus AppealStatus { get; set; }
        public int AdminId { get; set; }
        public User Admin { get; set; }
    }
}
