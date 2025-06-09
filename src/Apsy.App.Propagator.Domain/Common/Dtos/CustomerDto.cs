namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class CustomerDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public AddressDto Address { get; set; }

        public long Balance { get; set; }

        public DateTime Created { get; set; }

        public string Currency { get; set; }

        public bool? Deleted { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string InvoicePrefix { get; set; }

        public bool Livemode { get; set; }

        public string Name { get; set; }
    }
}
