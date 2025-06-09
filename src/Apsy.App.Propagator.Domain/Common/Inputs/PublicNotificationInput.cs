namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PublicNotificationInput : BaseInputDef
{

    public string Text { get; set; }
    public Gender Gender { get; set; }
    public int FromAge { get; set; }
    public int ToAge { get; set; }
    public bool IsSendAll { get; set; }

}