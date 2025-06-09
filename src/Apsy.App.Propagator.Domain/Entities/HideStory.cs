namespace Apsy.App.Propagator.Domain.Entities;

public class HideStory : EntityDef
{
    public User Hider { get; set; }
    public int HiderId { get; set; }

    public User Hided { get; set; }
    public int HidedId { get; set; }
}