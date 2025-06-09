using System.ComponentModel.DataAnnotations;
namespace Apsy.App.Propagator.Application.Common.Inputs;

public class WarningBannerInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Description { get; set; }

    public int? PostId { get; set; }
    public int? ArticleId { get; set; }

    [GraphQLIgnore]
    public bool IsActive { get; set; }
}