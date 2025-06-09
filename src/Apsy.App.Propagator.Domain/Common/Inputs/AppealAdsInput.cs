namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class AppealAdsInput : InputDef
{
    public int AdsId { get; set; }
    public string Description { get; set; }
    [GraphQLIgnore]
    public int UserId { get; set; }
}
