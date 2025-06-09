using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class ResetPasswordWithSecurityAnswerInput : InputDef
{
    [Required(ErrorMessage = "{0} is  Required")]
    [Display(Name = "Username")]
    public string Username { get; set; }
    public string Answer1 { get; set; }
    public int QuestionId1 { get; set; }
    public string Answer2 { get; set; }
    public int QuestionId2 { get; set; }
    public string Password { get; set; }
}