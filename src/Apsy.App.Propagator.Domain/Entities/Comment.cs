//using Aps.CommonBack.Social.Post.Comment.Models.Entities;

namespace Apsy.App.Propagator.Domain.Entities;

public class Comment : EntityDef /*: CommentDef<Post, User, Comment>*/
{

    #region
    public int PostId { get; set; }

    public Post Post { get; set; }
    public DeletedBy DeletedBy { get; set; }
    public int UserId { get; set; }

    public User User { get; set; }
    public int? MentionId { get; set; }

    public User Mention { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public int? ParentId { get; set; }

    public Comment Parent { get; set; }

    public ICollection<Comment> Children { get; set; }

    #endregion



    public string Text { get; set; }
    public CommentType CommentType { get; set; }
    public string ContentAddress { get; set; }
    public bool IsEdited { get; set; }
    public int LikeCount { get; set; }
    public ICollection<LikeComment> LikeComments { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public List<Report> Reports { get; set; }
    public int? ParentCommentId { get; set; } = 1;

 

    [GraphQLIgnore]
    public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser)
    {
        var commentRemovedEvent = new CommentRemovedEvent()
        {
            AdminId = currrentUser.Id,
            PostOwnerEmail = Post?.Poster?.Email,
            PostOwnerId = Post?.PosterId,
            YourMind = Post.YourMind,
            CommentId = Id,
            PostId = PostId,
            CommentText = Text,
            CommentContentAddress = ContentAddress,
            CommentType = CommentType

        };
        events.Add(commentRemovedEvent);
        return events;
    }

}

public class testComment  /*: CommentDef<Post, User, Comment>*/
{




    public string Text { get; set; }
    public CommentType CommentType { get; set; }
    public string ContentAddress { get; set; }
    public bool IsEdited { get; set; }
    public int LikeCount { get; set; }

}
