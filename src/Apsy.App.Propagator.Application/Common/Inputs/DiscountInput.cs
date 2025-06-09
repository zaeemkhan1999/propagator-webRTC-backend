namespace Apsy.App.Propagator.Application.Common.Inputs;

public class DiscountInput : BaseInputDef
{

    public string DiscountCode { get; set; }
    public int Percent { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpireDate { get; set; }

}