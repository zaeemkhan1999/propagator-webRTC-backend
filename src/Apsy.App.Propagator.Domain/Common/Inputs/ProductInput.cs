using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Domain.Common.Inputs
{
   public class ProductInput:InputDef
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int SellerId { get; set; }
        public string Description { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "At least 3 images are required.")]
        [MaxLength(10, ErrorMessage = "No more than 10 images are allowed.")]
        [GraphQLType(typeof(NonNullType<UploadType>))]
        public List<IFormFile> Images { get; set; }

        [Required]
        public float  Price { get; set; }

        [Required]
        public string Currency { get; set; }

        public int Stock { get; set; }


    }

    public class UpdateProductInput : InputDef
    {
        [Required]
        public int Id { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }


        
        [GraphQLType(typeof(NonNullType<UploadType>))]
        public List<IFormFile> Images { get; set; }


        public float  Price { get; set; }


        public string Currency { get; set; }

        public int Stock { get; set; }
        public List<int> ProductImageIds { get; set; }
    }
}
