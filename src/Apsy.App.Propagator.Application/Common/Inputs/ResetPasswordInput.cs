using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ResetPasswordInput : InputDef
{

    [Required(ErrorMessage = "{0} is  Required")]
    [EmailAddress(ErrorMessage = "invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    [MinLength(6, ErrorMessage = "The minimum {0} length is {1} characters")]
    [MaxLength(20, ErrorMessage = "The maximum {0} length is {1} characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }


    [Required(ErrorMessage = "{0} is  Required")]
    [Display(Name = "code")]
    public string Code { get; set; }
}
