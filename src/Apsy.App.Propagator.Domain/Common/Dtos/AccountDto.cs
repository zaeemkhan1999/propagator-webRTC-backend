namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class AccountDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public string BusinessType { get; set; }

        public bool ChargesEnabled { get; set; }

        public string Country { get; set; }

        public DateTime Created { get; set; }

        public string DefaultCurrency { get; set; }

        public bool? Deleted { get; set; }

        public bool DetailsSubmitted { get; set; }

        public string Email { get; set; }

        public bool PayoutsEnabled { get; set; }

        public string Type { get; set; }
    }
}
