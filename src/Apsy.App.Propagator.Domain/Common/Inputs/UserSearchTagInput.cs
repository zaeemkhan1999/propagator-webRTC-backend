using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class UserSearchTagInput : InputDef
{
    [GraphQLIgnore]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Tag { get; set; }
}