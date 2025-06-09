using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class LoginInput : InputDef
{
    [Required(ErrorMessage = "{0} is  Required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string Password { get; set; }
}
