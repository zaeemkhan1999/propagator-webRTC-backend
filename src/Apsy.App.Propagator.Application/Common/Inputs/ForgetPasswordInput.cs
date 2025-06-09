using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ForgetPasswordInput : InputDef
{
    [Required(ErrorMessage = "{0} is  Required")]
    [EmailAddress(ErrorMessage = "invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; }
}