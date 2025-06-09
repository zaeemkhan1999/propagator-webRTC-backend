namespace Apsy.App.Propagator.Domain.Entities;

public class Payment : EntityDef
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public double Amount { get; set; }
    public double AmountWithoutDiscount { get; set; }
    public double Discount { get; set; }

    public User User { get; set; }
    public int? UserId { get; set; }

    public string PaymentIntentId { get; set; }
    public string TransferId { get; set; }
    public PaymentConfirmationStatus PaymentConfirmationStatus { get; set; }
    public PaymentStatus PaymentStatus { get; set; }


    public bool IsDeletedAccount { get; set; }
    public DateTime DeleteAccountDate { get; set; }

    public Ads Ads { get; set; }
    public int? AdsId { get; set; }

    public int? UsersSubscriptionId { get; set; }
    public UsersSubscription UsersSubscription { get; set; }
}