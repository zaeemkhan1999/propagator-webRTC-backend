using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SecurityAnswerInput : InputDef
{
    public int? Id { get; set; }

    public int QuestionId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string Answer { get; set; }



}