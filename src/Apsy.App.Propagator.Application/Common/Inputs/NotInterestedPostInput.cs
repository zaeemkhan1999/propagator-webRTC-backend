using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class NotInterestedPostInput : BaseInputDef
{
    [GraphQLIgnore]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int? PostId { get; set; }
}