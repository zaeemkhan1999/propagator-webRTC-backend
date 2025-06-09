namespace Apsy.App.Propagator.Application.Common;

public class SubscriptionPlanDto : DtoDef
{
    public int Id { get; set; }
    public double Price { get; set; }
    public string PriceId { get; set; }
    public string Title { get; set; }
    public bool Supportbadge { get; set; }
    public bool RemoveAds { get; set; }
    public bool AllowDownloadPost { get; set; }
    public bool AddedToCouncilGroup { get; set; }
    public SubscriptionPlanContentDto Content { get; set; }
}