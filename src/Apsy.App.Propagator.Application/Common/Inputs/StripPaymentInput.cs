namespace Apsy.App.Propagator.Application.Common.Inputs;

public class StripPaymentInput : InputDef/*: StripeBasePaymentInputDef*/
{
    public double Amount { get; set; }

    public string StripeToken { get; set; }

    public string CustomerId { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}