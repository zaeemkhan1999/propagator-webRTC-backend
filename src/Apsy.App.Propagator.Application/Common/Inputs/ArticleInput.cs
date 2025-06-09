using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ArticleInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public bool IsByAdmin { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string Author { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string SubTitle { get; set; }

    public bool IsCreatedInGroup { get; set; }
    [GraphQLIgnore]
    public ArticleType ArticleType { get; set; }

    [GraphQLIgnore]
    public int UserId { get; set; }

    public List<ArticleItemInput> ArticleItems { get; set; }

    public List<LinkInput> LinkInputs { get; set; }
}