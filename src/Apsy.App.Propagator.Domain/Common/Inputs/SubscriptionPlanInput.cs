namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SubscriptionPlanInput : InputDef
{
    public string PriceId { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int DurationDays { get; set; }
}
