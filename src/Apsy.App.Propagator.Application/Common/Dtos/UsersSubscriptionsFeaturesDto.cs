using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Application.Common;

public class UsersSubscriptionsFeaturesDto : DtoDef
{
    public int SubscriptionPlanId { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool Supportbadge { get; set; }
    public bool RemoveAds { get; set; }
    public bool AllowDownloadPost { get; set; }
    public bool AddedToCouncilGroup { get; set; }
}