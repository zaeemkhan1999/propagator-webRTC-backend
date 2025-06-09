using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PromotePostInput : BasePromoteInput
{
    [Required(ErrorMessage = "{0} is required")]
    public int PostId { get; set; }
    public string DiscountCode { get; set; }
    public bool IsWithOutPayment { get; set; }

}