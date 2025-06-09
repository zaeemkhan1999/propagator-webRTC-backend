using System.ComponentModel.DataAnnotations;
namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class HighLightInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public string Title { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string Cover { get; set; }
    [GraphQLIgnore]
    public int? UserId { get; set; }
    public List<int> StoryIds { get; set; }
}