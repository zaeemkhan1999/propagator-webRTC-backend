namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class TransferDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public long Amount { get; set; }

        public long AmountReversed { get; set; }

        public string BalanceTransactionId { get; set; }

        public DateTime Created { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public string DestinationId { get; set; }

        public AccountDto Destination { get; set; }

        public string DestinationPaymentId { get; set; }

        public ChargeDto DestinationPayment { get; set; }

        public bool Livemode { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public bool Reversed { get; set; }

        public string SourceTransactionId { get; set; }

        public ChargeDto SourceTransaction { get; set; }

        public string SourceType { get; set; }

        public string TransferGroup { get; set; }
    }
}
