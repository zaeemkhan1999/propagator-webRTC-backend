namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class NotificationInput : BaseInputDef
{
    public string Text { get; set; }
    public NotificationType NotificationType { get; set; }

    public Gender? Gender { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
