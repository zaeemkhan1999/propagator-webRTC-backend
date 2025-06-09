using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ArticleItemInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public string Data { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public int? Order { get; set; }
    public string VideoTime { get; set; }
    public string VideoShape { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public ArticleItemType? ArticleItemType { get; set; }
}