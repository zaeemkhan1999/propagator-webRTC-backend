using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class LoginInput : InputDef
{
    [Required(ErrorMessage = "{0} is  Required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string Password { get; set; }
}
