namespace Apsy.App.Propagator.Application.Common
{
    public class ProductDto
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public bool Active { get; set; }

        public string Caption { get; set; }

        public DateTime Created { get; set; }

        public string DefaultPriceId { get; set; }

        public bool? Deleted { get; set; }

        public string Description { get; set; }

        public List<string> Images { get; set; }

        public bool Livemode { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public string Name { get; set; }

        public bool? Shippable { get; set; }

        public string StatementDescriptor { get; set; }

        public string Type { get; set; }

        public string UnitLabel { get; set; }

        public DateTime Updated { get; set; }

        public string Url { get; set; }
    }
}
