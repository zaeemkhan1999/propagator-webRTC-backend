namespace Apsy.App.Propagator.Domain.Entities;

public class Message : MessageDef<User, Conversation>
{
    public Message()
    {
    }

    public MessageType MessageType { get; set; }
    public string ContentAddress { get; set; }
    public int? GroupId { get; set; }
    public bool IsEdited { get; set; } = false;
    public bool IsSeen { get; set; }
    public bool IsDelivered { get; set; }

    public DateTime? SeenDate { get; set; }

    public bool IsShare { get; set; }

    public int? ReceiverId { get; set; }
    public User Receiver { get; set; }

    public int? ProfileId { get; set; }
    public User Profile { get; set; }

    public Post Post { get; set; }
    public int? PostId { get; set; }

    public Article Article { get; set; }
    public int? ArticleId { get; set; }

    public Story Story { get; set; }
    public int? StoryId { get; set; }

    public GroupTopic GroupTopic { get; set; }
    public int? GroupTopicId { get; set; }

    public Message ParentMessage { get; set; }
    public int? ParentMessageId { get; set; }

    public List<Message> ChildrenMessages { get; set; }
    
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<Report> Reports { get; set; }

}