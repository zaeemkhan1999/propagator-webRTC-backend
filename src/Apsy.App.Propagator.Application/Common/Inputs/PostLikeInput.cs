namespace Apsy.App.Propagator.Application.Common.Inputs;

public class PostLikeInput : BaseInputDef
{
    public bool? Liked { get; set; }
    public int? UserId { get; set; }
    public int? PostId { get; set; }
}
