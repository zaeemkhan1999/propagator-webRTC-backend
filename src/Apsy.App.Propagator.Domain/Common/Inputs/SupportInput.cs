using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SupportInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Text { get; set; }
    public string Category { get; set; }


    [GraphQLIgnore]
    public int? UserId { get; set; }
}
