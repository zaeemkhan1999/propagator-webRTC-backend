namespace Apsy.App.Propagator.Application.Common.Inputs;

public class RegisterWithSecurityAnswerInput : InputDef
{
    public string Answer { get; set; }

    public int QuestionId { get; set; }
}
