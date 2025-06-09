namespace Apsy.App.Propagator.Application.Common
{
    public class ArticleLikeDto : DtoDef
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Article Article { get; set; }
        public int ArticleId { get; set; }

        public bool IsViewed { get; set; }
        public bool IsSaved { get; set; }
        public bool IsNotInterested { get; set; }
        public bool IsLiked { get; set; }
        public bool IsYourArticle { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string ArticleItemsString { get; set; }
    }
}
