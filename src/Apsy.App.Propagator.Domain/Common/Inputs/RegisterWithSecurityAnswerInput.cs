namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class RegisterWithSecurityAnswerInput : InputDef
{
    public string Answer { get; set; }

    public int QuestionId { get; set; }
}
