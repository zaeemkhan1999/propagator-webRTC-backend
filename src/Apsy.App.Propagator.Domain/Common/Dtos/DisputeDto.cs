namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class DisputeDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public long? Amount { get; set; }

        public string ChargeId { get; set; }

        public ChargeDto Charge { get; set; }

        public DateTime Created { get; set; }

        public string Currency { get; set; }

        public bool IsChargeRefundable { get; set; }

        public bool Livemode { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public string NetworkReasonCode { get; set; }

        public string PaymentIntentId { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }
    }
}
