using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PostAdsInput : BasePromoteInput /*PostInputDef*/
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string YourMind { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public bool AllowDownload { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Location { get; set; }

    public int? PosterId { get; set; }


    [GraphQLIgnore]
    public PostType PostType { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public List<PostItemInput> PostItems { get; set; }

    public List<string> Tags { get; set; }

    [GraphQLIgnore]
    public string StringTags { get; set; }

    public bool IsWithOutPayment { get; set; }
    public string DiscountCode { get; set; }
    public IconLayoutType IconLayoutType { get; set; }

}
