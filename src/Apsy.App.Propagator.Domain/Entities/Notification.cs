

namespace Apsy.App.Propagator.Domain.Entities;

public class Notification : EntityDef/*: UserKindDef<User>*/
{
    public string Text { get; set; }
    public bool IsReaded { get; set; }
    public NotificationType NotificationType { get; set; }

    public UserFollower UserFollower { get; set; }
    public int? UserFollowerId { get; set; }

    public PostLikes PostLike { get; set; }
    public int? PostLikeId { get; set; }

    public Post Post { get; set; }
    public int? PostId { get; set; }

    public Article Article { get; set; }
    public int? Articled { get; set; }

    public Comment Comment { get; set; }
    public int? CommentId { get; set; }

    public LikeComment LikeComment { get; set; }
    public int? LikeCommentId { get; set; }

    public ArticleComment ArticleComment { get; set; }
    public int? ArticleCommentId { get; set; }

    public LikeArticleComment LikeArticleComment { get; set; }
    public int? LikeArticleCommentId { get; set; }

    public ArticleLike ArticleLike { get; set; }
    public int? ArticleLikeId { get; set; }

    public StoryLike StoryLike { get; set; }
    public int? StoryLikeId { get; set; }    
    
    public Ads Ads { get; set; }
    public int? AdsId { get; set; }


    public StoryComment StoryComment { get; set; }
    public int? StoryCommentId { get; set; }


    public Message Message { get; set; }
    public int? MessageId { get; set; }


    public User Sender { get; set; }
    public int? SenderId { get; set; }

    public User Reciever { get; set; }
    public int RecieverId { get; set; }

    public Conversation Conversation { get; set; }
    public int? ConversationId { get; set; }
}
