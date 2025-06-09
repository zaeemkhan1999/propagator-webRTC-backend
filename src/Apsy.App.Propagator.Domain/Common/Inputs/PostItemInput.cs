using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public record PostItemInput /*: BaseInputDef*/
{
    [Required(ErrorMessage = "{0} is required")]
    public int? Order { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string ThumNail { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string Content { get; set; }
    public string SummaryVideoLink { get; set; }
    public string VideoShape { get; set; }
    public string VideoTime { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public PostItemType PostItemType { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}