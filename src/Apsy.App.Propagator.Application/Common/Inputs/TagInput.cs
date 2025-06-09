using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class TagInput : BaseInputDef
{

    [Required(ErrorMessage = "{0} is required")]
    public string Tag { get; set; }

}
