namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class StripFullPaymentInput : InputDef /*: StripFullPaymentInputDef*/
{

    public string CardNumder { get; set; }

    public string CVC { get; set; }

    public long ExpMonth { get; set; }

    public long ExpYear { get; set; }

    public long Amount { get; set; }

    public string Currency { get; set; } = "usd";


    public string Description { get; set; }

    public string CustomerId { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}