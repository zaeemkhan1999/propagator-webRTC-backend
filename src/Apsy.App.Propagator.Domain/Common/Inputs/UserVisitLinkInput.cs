using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class UserVisitLinkInput : InputDef
{
    public int? PostId { get; set; }
    public int? ArticleId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public LinkType LinkType { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string Text { get; set; }
    public string Link { get; set; }

    [GraphQLIgnore]
    public int UserId { get; set; }
}