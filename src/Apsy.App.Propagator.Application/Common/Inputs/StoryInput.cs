using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class StoryInput : BaseInputDef
{
    public string ContentAddress { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public StoryType StoryType { get; set; }
    public string Link { get; set; }
    public string Text { get; set; }
    [GraphQLIgnore]
    public int? UserId { get; set; }
    public int? HighLightId { get; set; }
    public int? PostId { get; set; }
    public int? ArticleId { get; set; }
    public int? Duration { get; set; }
}