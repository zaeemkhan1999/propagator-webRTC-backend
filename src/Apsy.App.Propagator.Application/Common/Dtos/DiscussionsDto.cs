namespace Apsy.App.Propagator.Application.Common
{
    public class DiscussionsDto : DtoDef
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public MessageType MessageType { get; set; }

        public Post Post { get; set; }
        public Article Article { get; set; }
        public int? GroupTopicId { get; set; }

        public bool IsViewed { get; set; }
        public bool IsSaved { get; set; }
        public bool IsNotInterested { get; set; }
        public bool IsLiked { get; set; }
        public bool IsYours { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string ItemsString { get; set; }
    }
}