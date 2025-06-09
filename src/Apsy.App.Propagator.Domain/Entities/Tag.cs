namespace Apsy.App.Propagator.Domain.Entities;

public class Tag : EntityDef
{
    public string Text { get; set; }
    public int Hits { get; set; }
    public List<UserViewTag> UserViewTags { get; set; }
    public int UsesCount { get; set; }
    public int LikesCount { get; set; }
}