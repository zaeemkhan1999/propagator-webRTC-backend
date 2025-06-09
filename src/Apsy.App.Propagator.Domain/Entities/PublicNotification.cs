

namespace Apsy.App.Propagator.Domain.Entities;

public class PublicNotification : EntityDef
{
    public string Text { get; set; }
    public int FromAge { get; set; }
    public int ToAge { get; set; }
    public Gender Gender { get; set; }
    public bool IsSendAll { get; set; }
}