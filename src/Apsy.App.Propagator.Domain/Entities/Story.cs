namespace Apsy.App.Propagator.Domain.Entities;

public class Story : UserKindDef<User>
{
    public string textPositionX { get; set; } //Updated new field
    public string textPositionY { get; set; } //Updated new field
    public string textStyle { get; set; } //Updated new field
    public string ContentAddress { get; set; }
    public StoryType StoryType { get; set; }
    public string Link { get; set; }
    public string Text { get; set; }
    public DeletedBy DeletedBy { get; set; }

    public HighLight HighLight { get; set; }
    public int? HighLightId { get; set; }

    public Post Post { get; set; }
    public int? PostId { get; set; }

    public Article Article { get; set; }
    public int? ArticleId { get; set; }

    public int? Duration { get; set; }

    public ICollection<StoryComment> StoryComments { get; set; }
    public ICollection<StoryLike> StoryLikes { get; set; }
    public ICollection<StorySeen> StorySeens { get; set; }
    public bool LikedByCurrentUser { get; set; }
    public bool SeenByCurrentUser { get; set; } 
    public ICollection<Message> Messages { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}
