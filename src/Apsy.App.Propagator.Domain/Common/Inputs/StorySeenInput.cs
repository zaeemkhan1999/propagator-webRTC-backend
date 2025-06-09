namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class StorySeenInput : BaseInputDef
{
    public int? StoryId { get; set; }
    [GraphQLIgnore]
    public int UserId { get; set; }
    public int OwnerId { get; set; }
}