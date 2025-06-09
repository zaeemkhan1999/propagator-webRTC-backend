using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class UserInput : BaseInputDef
{
    public string Bio { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string DisplayName { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string Username { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public DateTime? DateOfBirth { get; set; }
    public string ImageAddress { get; set; }
    public string Cover { get; set; }
    public Gender Gender { get; set; }
    public string LinkBio { get; set; }
    public string Location { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public bool EnableTwoFactorAuthentication { get; set; }
    public string PhoneNumber { get; set; }
    public string CountryCode { get; set; }
}