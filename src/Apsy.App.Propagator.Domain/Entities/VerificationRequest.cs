

namespace Apsy.App.Propagator.Domain.Entities;
public class VerificationRequest : UserKindDef<User>
{
    public VerificationRequestAcceptStatus VerificationRequestAcceptStatus { get; set; }
    public string ProofOfAddress { get; set; }
    public string GovernmentIssuePhotoId { get; set; }
    public List<string> OtheFiles { get; set; }
    public string ReasonReject { get; set; }
}