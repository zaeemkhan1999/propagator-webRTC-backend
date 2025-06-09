using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class StoryInput : BaseInputDef
{
    public string textPositionX { get; set; } //Updated new field
    public string textPositionY { get; set; } //Updated new field
    public string textStyle { get; set; } //Updated new field
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