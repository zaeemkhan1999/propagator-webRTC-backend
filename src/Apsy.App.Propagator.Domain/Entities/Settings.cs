namespace Apsy.App.Propagator.Domain.Entities
{
    public class Settings : EntityDef
    {
        public int TotalPostsCount { get; set; }
        public int TotalArticlesCount { get; set; }
        public int TotalPostCommentsCount { get; set; }
        public int TotalArticleCommentsCount { get; set; }
        public int TotalTagsCount { get; set; }
        public int TotalTagsViews { get; set; }
        public int TotalPostLikesCount { get; set; }
        public int TotalArticleLikesCount { get; set; }
        public int TotalPromotedPostsCount { get; set; }
        public int TotalPromotedArticlesCount { get; set; }
        public int TotalAdsesCount { get; set; }
        public int TotalStoriesCount { get; set; }
    }
}
