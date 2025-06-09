using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Domain.Entities
{
   public class ProductImages:EntityDef
    {

        [Required]
        public int ProductId { get; set; } 

        [Required]
        [Url]
        public string ImageUrl { get; set; } 
    }
}
