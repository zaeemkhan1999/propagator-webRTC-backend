namespace Apsy.App.Propagator.Domain.Common.Dtos;

public class SubscriptionPlanContentDto : DtoDef
{
    public string Duration { get; set; }
    public List<string> Features { get; set; }

    public static implicit operator string(SubscriptionPlanContentDto v)
    {
        throw new NotImplementedException();
    }
}