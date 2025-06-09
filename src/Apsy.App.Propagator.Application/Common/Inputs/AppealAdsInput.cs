namespace Apsy.App.Propagator.Application.Common.Inputs;

public class AppealAdsInput : InputDef
{
    public int AdsId { get; set; }
    public string Description { get; set; }
    [GraphQLIgnore]
    public int UserId { get; set; }
}
