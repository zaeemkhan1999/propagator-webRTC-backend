using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Entities
{
    public class Product:EntityDef
    {
        [Required]
        public string Name { get; set; }
        public int SellerId { get; set; }
        public string Description { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "At least 3 images are required.")]
        [MaxLength(10, ErrorMessage = "No more than 10 images are allowed.")]
        [GraphQLIgnore]
        [JsonIgnore]
        public virtual List<ProductImages> Images { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public int Stock { get; set; }

        public List<Reviews> Review { get; set; }
        public User Seller { get; set; }
    }
}

