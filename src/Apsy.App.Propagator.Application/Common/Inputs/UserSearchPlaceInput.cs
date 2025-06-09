namespace Apsy.App.Propagator.Application.Common.Inputs;

public class UserSearchPlaceInput
 : BaseInputDef
{

    public string Place { get; set; }

    [GraphQLIgnore]
    public int? UserId { get; set; }

}
