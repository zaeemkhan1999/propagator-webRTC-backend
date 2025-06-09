using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class PostInput : InputDef /*PostInputDef*/
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public bool IsByAdmin { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string YourMind { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public bool AllowDownload { get; set; }

    public string Location { get; set; }

    public int? PosterId { get; set; }

    public bool IsCreatedInGroup { get; set; }

    [GraphQLIgnore]
    public PostType PostType { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public List<PostItemInput> PostItems { get; set; }

    public List<string> Tags { get; set; }

    [GraphQLIgnore]
    public string StringTags { get; set; }

    public List<LinkInput> LinkInputs { get; set; }

    public IconLayoutType IconLayoutType { get; set; }
}