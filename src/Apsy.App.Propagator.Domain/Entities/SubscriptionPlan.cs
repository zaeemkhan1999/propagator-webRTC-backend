namespace Apsy.App.Propagator.Domain.Entities;

public class SubscriptionPlan : EntityDef
{
    /// <summary>
    /// The priceId of the defined product in the stripe panel
    /// </summary>
    public string PriceId { get; set; }

    /// <summary>
    /// Value of plan to display by frontend
    /// </summary>
    public double Price { get; set; }

    public bool IsActive { get; set; }
    public string Title { get; set; }

    /// <summary>
    /// Text and every content that is going to display in plans page in the app for each plan
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// How many days user can use this package per each buy
    /// </summary>
    public int DurationDays { get; set; }

    public bool Supportbadge { get; set; }
    public bool RemoveAds { get; set; }
    public bool AllowDownloadPost { get; set; }
    public bool AddedToCouncilGroup { get; set; }

    public ICollection<UsersSubscription> UsersSubscriptions { get; set; }
}
