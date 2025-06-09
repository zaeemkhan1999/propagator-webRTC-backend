using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class NotInterestedArticleInput : BaseInputDef
{
    [GraphQLIgnore]
    public int? UserId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public int? ArticleId { get; set; }
}