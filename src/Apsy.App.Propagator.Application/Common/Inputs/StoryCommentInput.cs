using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class StoryCommentInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public string Text { get; set; }

    [GraphQLIgnore]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int? StoryId { get; set; }
}