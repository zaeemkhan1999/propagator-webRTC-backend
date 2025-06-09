namespace Apsy.App.Propagator.Application.Common
{
    public class ChargeDto
    {
        public string Id { get; set; }

        public long Amount { get; set; }

        public long AmountCaptured { get; set; }

        public long AmountRefunded { get; set; }

        public string ApplicationId { get; set; }

        public string ApplicationFeeId { get; set; }

        public long? ApplicationFeeAmount { get; set; }

        public string BalanceTransactionId { get; set; }

        public string Currency { get; set; }

        public string CustomerId { get; set; }

        public string Description { get; set; }

        public bool Disputed { get; set; }

        public string FailureCode { get; set; }

        public string FailureMessage { get; set; }

        public string InvoiceId { get; set; }

        public bool Livemode { get; set; }

        public string ReceiptEmail { get; set; }

        public string ReceiptNumber { get; set; }

        public string ReceiptUrl { get; set; }
    }
}
