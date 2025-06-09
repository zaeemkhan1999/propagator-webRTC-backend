namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PaymentInput : BaseInputDef
{

    public double Amount { get; set; }
    public int UserId { get; set; }

    //public PaymentType PaymentType { get; set; }
    //public PaymentConfirmationStatus PaymentConfirmationStatus { get; set; }

}
