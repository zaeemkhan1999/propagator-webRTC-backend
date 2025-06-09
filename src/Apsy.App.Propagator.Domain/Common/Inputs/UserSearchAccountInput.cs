using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class UserSearchAccountInput : InputDef
{
    [GraphQLIgnore]
    public int? SearcherId { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public int? SearchedId { get; set; }
    public string SearchedName { get; set; }
}