namespace Apsy.App.Propagator.Domain.Entities;

public class Conversation : ConversationDef<User, Message>
{
    public Conversation()
    {
        IsFirstUserDeleted = false;
        IsSecondUserDeleted = false;
    }


    public new int? FirstUserId { get; set; }
    public new int? SecondUserId { get; set; }




    public bool IsFirstUserDeleted { get; set; }
    public DateTime FirstUserDeletedDate { get; set; }

    public bool IsSecondUserDeleted { get; set; }
    public DateTime SecondUserDeletedDate { get; set; }


    public int LatestMessageUserId { get; set; }

    /// <summary>
    /// This is admin of the Group
    /// </summary>
    public User Admin { get; set; }
    public int? AdminId { get; set; }

    public bool IsGroup { get; set; }
    public string GroupName { get; set; }
    public string GroupDescription { get; set; }
    public string GroupLink { get; set; }
    public string GroupImgageUrl { get; set; }
    public bool IsPrivate { get; set; }
    public new DateTime? LatestMessageDate { get; set; } = DateTime.UtcNow;

    public int? SubscriptionPlanId { get; set; }

    public ICollection<UserMessageGroup> UserGroups { get; set; }
    public ICollection<UserSearchGroup> UserSearchGroups { get; set; }
    public ICollection<GroupTopic> GroupTopics { get; set; }
}