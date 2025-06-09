using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class LoginWithSecurityAnswerInput : InputDef
{

    [Required(ErrorMessage = "{0} is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Answer1 { get; set; }

    public int QuestionId1 { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Answer2 { get; set; }

    public int QuestionId2 { get; set; }
}
