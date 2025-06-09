namespace Apsy.App.Propagator.Application.Common.Inputs;

public class BlockUserInput : InputDef
{
    [GraphQLIgnore]
    public int? BlockerId { get; set; }
    public int? BlockedId { get; set; }
}