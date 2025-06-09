namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class InterestedUserInput : BaseInputDef
{

    public int FollowersUserId { get; set; }
    public int UserId { get; set; }
    public InterestedUserType InterestedUserType { get; set; }
    public int? PostId { get; set; }
    public int? ArticleId { get; set; }

}