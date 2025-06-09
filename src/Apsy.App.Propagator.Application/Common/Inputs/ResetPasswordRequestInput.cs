using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ResetPasswordRequestInput
{
    [Required(ErrorMessage = "{0} is  Required")]
    public string GovernmentIssuePhotoId { get; set; }

    public List<string> OtherFiles { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string LegalName { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string DisplayName { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string ContactEmailOrUsername { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string ProofOfAddress { get; set; }

    [GraphQLIgnore]
    public ResetPasswordRequestStatus Status { get; set; }
}
