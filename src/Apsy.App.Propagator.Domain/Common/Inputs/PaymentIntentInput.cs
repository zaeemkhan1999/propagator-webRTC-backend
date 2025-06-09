namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PaymentIntentInput : InputDef
{
    public PaymentIntentInput(string currency, double amount, string customer, string receiptEmail, Dictionary<string, string> metaData)
    {
        Currency = currency;
        Amount = amount;
        Customer = customer;
        ReceiptEmail = receiptEmail;
        MetaData = metaData;
    }

    public string Currency { get; set; }
    public double Amount { get; set; }
    public string Customer { get; set; }
    public string ReceiptEmail { get; set; }
    public Dictionary<string, string> MetaData { get; set; }
}
