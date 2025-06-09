namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class NotificationDto : DtoDef
    {
        public Notification Notification { get; set; }
        public UserFollower IFollowUser { get; set; }

        // Required for deletion checks
        public Post RelatedPost { get; set; }
        public Comment RelatedComment { get; set; }

        public Article RelatedArticle {get; set;}  

        public PostLikes RelatedPostLikes {get; set;}

    
    // Add calculated properties if needed
    public bool IsValid => 
        (RelatedPost == null || !RelatedPost.IsDeleted) &&
        (RelatedComment == null || (!RelatedComment.IsDeleted && 
                                   !RelatedComment.Post.IsDeleted)) &&
        (RelatedArticle == null || !RelatedArticle.IsDeleted) &&
        (RelatedPostLikes == null || !RelatedPostLikes.IsDeleted);
        //public List<UserInfoDto> PeopleWhoLikePost { get; set; }
        //public List<UserInfoDto> PeopleWhoNewComment { get; set; }
        //public List<UserInfoDto> PeopleWhoNewReplyComment { get; set; }
        //public List<UserInfoDto> PeopleWhoLikeComment { get; set; }
        //public List<UserInfoDto> PeopleWhoArticleComment { get; set; }
        //public List<UserInfoDto> PeopleWhoNewReplyToArticleComment { get; set; }
        //public List<UserInfoDto> PeopleWhoLikeArticleComment { get; set; }
        //public List<UserInfoDto> PeopleWhoArticleLike { get; set; }
    }


    public class PostNotificationDto : DtoDef
    {
        public Post Post { get; set; }
        public bool HasUnreadNotifications { get; set; }
        public IQueryable<UserInfoDto> Users { get; set; }
    }




}
