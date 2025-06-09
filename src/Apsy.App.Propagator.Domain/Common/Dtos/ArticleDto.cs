namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ArticleDto
    {
        public Article Article { get; set; }
        public bool IsViewed { get; set; }
        public bool IsSaved { get; set; }
        public bool IsNotInterested { get; set; }
        public bool IsLiked { get; set; }
        public bool IsYourArticle { get; set; }
        public int CommentCount { get; set; }
        [GraphQLIgnore]
        public bool IsInterested { get; set; }
        public int ShareCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int NotInterestedArticlesCount { get; set; }
        public string ArticleItemsString { get; set; }
        public List<ArticleCommentDto> ArticleComments { get; set; }
    }
}
