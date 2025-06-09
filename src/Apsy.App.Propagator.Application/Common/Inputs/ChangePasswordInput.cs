using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ChangePasswordInput : InputDef
{
    [Required(ErrorMessage = "{0} is  Required")]
    [MinLength(6, ErrorMessage = "The minimum {0} length is {1} characters")]
    [MaxLength(20, ErrorMessage = "The maximum {0} length is {1} characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    [MinLength(6, ErrorMessage = "The minimum {0} length is {1} characters")]
    [MaxLength(20, ErrorMessage = "The maximum {0} length is {1} characters")]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }
}
