using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class VerificationRequestInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public VerificationRequestAcceptStatus VerificationRequestAcceptStatus { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string ProofOfAddress { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string GovernmentIssuePhotoId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public List<string> OtheFiles { get; set; }
    [GraphQLIgnore]
    public int? UserId { get; set; }
}