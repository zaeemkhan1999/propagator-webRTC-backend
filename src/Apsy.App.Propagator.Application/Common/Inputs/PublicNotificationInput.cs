namespace Apsy.App.Propagator.Application.Common.Inputs;

public class PublicNotificationInput : BaseInputDef
{

    public string Text { get; set; }
    public Gender Gender { get; set; }
    public int FromAge { get; set; }
    public int ToAge { get; set; }
    public bool IsSendAll { get; set; }

}