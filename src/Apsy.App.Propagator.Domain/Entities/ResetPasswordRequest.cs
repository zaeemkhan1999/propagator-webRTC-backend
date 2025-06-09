namespace Apsy.App.Propagator.Domain.Entities;

public class ResetPasswordRequest : EntityDef
{
    public string GovernmentIssuePhotoId { get; set; }

    public List<string> OtherFiles { get; set; }

    public string Username { get; set; }
    
    public string LegalName { get; set; }

    public string DisplayName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string ContactEmailOrUsername { get; set; }

    public string ProofOfAddress { get; set; }

    public ResetPasswordRequestStatus Status { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
}