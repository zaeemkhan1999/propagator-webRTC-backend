using System.ComponentModel.DataAnnotations.Schema;

namespace Apsy.App.Propagator.Domain.Entities;

public class UsersSubscription : UserKindDef<User>
{
    public DateTime ExpirationDate { get; set; }
    public UserSubscriptionStatuses Status { get; set; }

    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }

    public int PaymentId { get; set; }
    public Payment Payment { get; set; }
}