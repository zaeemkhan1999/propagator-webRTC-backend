using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class UserSearchGroupInput : InputDef
{
    [GraphQLIgnore]
    public int? UserId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public int? ConversationId { get; set; }

}
