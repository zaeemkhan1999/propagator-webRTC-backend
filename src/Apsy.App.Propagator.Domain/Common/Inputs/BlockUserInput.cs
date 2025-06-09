namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class BlockUserInput : InputDef
{
    [GraphQLIgnore]
    public int? BlockerId { get; set; }
    public int? BlockedId { get; set; }
}