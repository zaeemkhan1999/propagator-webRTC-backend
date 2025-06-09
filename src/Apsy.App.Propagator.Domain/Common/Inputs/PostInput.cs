using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PostInput : InputDef /*PostInputDef*/
{
    public string Duration { get; set; }
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
    public string Bg { get; set; }
    public string AspectRatio { get; set; }
    
    
    [GraphQLIgnore]
    public PostType PostType { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public List<PostItemInput> PostItems { get; set; }
        
    public List<string> Tags { get; set; }
    [GraphQLIgnore]
    public string StringTags { get; set; }
    [GraphQLType(typeof(NonNullType<UploadType>))]

    //public IFormFile Thumbnail { get; set; }
    [GraphQLIgnore]
    public List<LinkInput> LinkInputs { get; set; }

    public IconLayoutType IconLayoutType { get; set; }
}