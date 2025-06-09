//using Aps.CommonBack.Payments.Models.Dtos;

namespace Apsy.App.Propagator.Application.Common
{
    public class PlanDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public bool Active { get; set; }

        public string AggregateUsage { get; set; }

        public long? Amount { get; set; }

        public decimal? AmountDecimal { get; set; }

        public string BillingScheme { get; set; }

        public DateTime Created { get; set; }

        public string Currency { get; set; }

        public bool? Deleted { get; set; }

        public string Interval { get; set; }

        public long IntervalCount { get; set; }

        public bool Livemode { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public string Nickname { get; set; }

        public string ProductId { get; set; }

        public string TiersMode { get; set; }

        public long? TrialPeriodDays { get; set; }

        public string UsageType { get; set; }

        public ProductDto Product { get; set; }
    }
}
