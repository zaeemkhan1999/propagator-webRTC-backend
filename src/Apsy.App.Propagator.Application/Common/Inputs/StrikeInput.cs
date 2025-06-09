using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class StrikeInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public string Text { get; set; }

    public int? PostId { get; set; }
    public int? ArticleId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int UserId { get; set; }
}