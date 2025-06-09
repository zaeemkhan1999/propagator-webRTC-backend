namespace Apsy.App.Propagator.Application.Common.Inputs;

public class StorySeenInput : BaseInputDef
{
    public int? StoryId { get; set; }
    [GraphQLIgnore]
    public int? UserId { get; set; }
}