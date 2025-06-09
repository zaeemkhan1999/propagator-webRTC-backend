namespace Apsy.App.Propagator.Domain.Entities;

public class StorySeen : UserKindDef<User>
{
    public Story Story { get; set; }
    public int StoryId { get; set; }
}
