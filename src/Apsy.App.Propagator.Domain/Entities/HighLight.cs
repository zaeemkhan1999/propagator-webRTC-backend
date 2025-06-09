namespace Apsy.App.Propagator.Domain.Entities;

public class HighLight : UserKindDef<User>
{
    public string Title { get; set; }
    public string Cover { get; set; }
    public List<Story> Stories { get; set; }
}