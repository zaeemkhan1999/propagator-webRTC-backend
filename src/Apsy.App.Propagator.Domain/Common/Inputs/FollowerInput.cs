
namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class FollowerInput : InputDef /*FollowerInputDef*/
{
    [GraphQLIgnore]
    public bool? IsMutual { get; set; }

    [GraphQLIgnore]
    public DateTime? FollowedAt { get; set; }

    [GraphQLIgnore]
    public FolloweAcceptStatus FolloweAcceptStatus { get; set; }

    public int? FollowerId { get; set; }

    public int? FollowedId { get; set; }

}