using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class GroupInput : InputDef
{
    public int ConversationId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public string GroupName { get; set; }

    public string GroupDescription { get; set; }

    public string GroupImgageUrl { get; set; }

    public string GroupLink { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public bool? IsPrivate { get; set; }
}