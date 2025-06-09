using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class LoginWithOTPInput : InputDef
{

    [Required(ErrorMessage = "{0} is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Code { get; set; }
}
