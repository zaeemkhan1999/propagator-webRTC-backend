using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ResendEmailConfirmationInput
{
    [Required(ErrorMessage = "{0} is required")]
    [EmailAddress(ErrorMessage = "invalid email address")]
    public string Email { get; set; }
}
