namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class UserSearchPlaceInput
 : BaseInputDef
{

    public string Place { get; set; }

    [GraphQLIgnore]
    public int? UserId { get; set; }

}
