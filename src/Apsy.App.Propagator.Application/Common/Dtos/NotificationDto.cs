namespace Apsy.App.Propagator.Application.Common
{
    public class NotificationDto : DtoDef
    {
        public Notification Notification { get; set; }
        public UserFollower IFollowUser { get; set; }

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
