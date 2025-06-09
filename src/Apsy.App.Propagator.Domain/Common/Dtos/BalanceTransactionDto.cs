namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class BalanceTransactionDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public long Amount { get; set; }

        public DateTime AvailableOn { get; set; }

        public DateTime Created { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public decimal? ExchangeRate { get; set; }

        public long Fee { get; set; }

        public long Net { get; set; }

        public string ReportingCategory { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }
    }
}
