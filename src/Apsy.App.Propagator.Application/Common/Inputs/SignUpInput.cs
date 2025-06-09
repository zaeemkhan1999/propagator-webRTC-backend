using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class SignUpInput : InputDef
{
    public string Email { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string DisplayName { get; set; }

    // [Required(ErrorMessage = "{0} is required")]
    // public string LegalName { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Username { get; set; }

    // [Required(ErrorMessage = "{0} is required")]
    // public DateTime? DateOfBirth { get; set; }

    // [Required(ErrorMessage = "{0} is required")]
    // public bool? EnableTwoFactorAuthentication { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public Gender Gender { get; set; }

    // public string Ip { get; set; }
}