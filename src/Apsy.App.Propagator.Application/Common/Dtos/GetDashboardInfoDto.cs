namespace Apsy.App.Propagator.Application.Common
{
    public class GetDashboardInfoDto : DtoDef
    {
        public int NumberOfPosts { get; set; }
        public double NumberOfPostsRatePercent { get; set; }

        public int NumberOfPostComments { get; set; }
        public double NumberOfPostCommentsRatePercent { get; set; }

        public int NumberOfPostLikes { get; set; }
        public double NumberOfPostLikesRatePercent { get; set; }


        public int NumberOfPostPromotions { get; set; }
        public double NumberOfPostPromotionsRatePercent { get; set; }


        public int NumberOfArticles { get; set; }
        public double NumberOfArticlesRatePercent { get; set; }

        public int NumberOfArticleComments { get; set; }
        public double NumberOfArticleCommentsRatePercent { get; set; }

        public int NumberOfArticleLikes { get; set; }
        public double NumberOfArticleLikesRatePercent { get; set; }


        public int NumberOfArticlePromotions { get; set; }

        public int NumberOfTags { get; set; }
        public double NumberOfTagsRatePercent { get; set; }



        public double PublicAccountPercent { get; set; }
        public double PrivateAccountPercent { get; set; }
        public double WomanAccountPercent { get; set; }
        public double ManAccountPercent { get; set; }
        public double RatherNotSayAccountPercent { get; set; }
        public double CustomAccountPercent { get; set; }

        public double AccountWith0To15AgePercent { get; set; }
        public double AccountWith15To20AgePercent { get; set; }
        public double AccountWith20To25AgePercent { get; set; }
        public double AccountWith25To30AgePercent { get; set; }
        public double AccountWith30To35AgePercent { get; set; }
        public double AccountWith35To40AgePercent { get; set; }
        public double AccountWith40To120AgePercent { get; set; }


        public Top5Tags Top5Tag { get; set; }
        public List<TopTagUsesCount> Top5TagUsesCounts { get; set; }
        public List<TopTagLikesCount> Top5TagLikesCounts { get; set; }
        public Settings TotalViewsInSystem { get; set; }

    }

    public class Top5Tags
    {
        public int TotalTagsView { get; set; }
        public List<TopTag> TopTags { get; set; }

    }
    public class TopTag
    {
        public int ViewCount { get; set; }
        public string Tag { get; set; }
    }

    public class TopTagUsesCount
    {
        public int UsesCount { get; set; }
        public string Tag { get; set; }
    }
    public class TopTagLikesCount
    {
        public int LikeCount { get; set; }
        public string Tag { get; set; }
    }

    public class DashboardInfo : DtoDef
    {

        public double PublicAccountPercent { get; set; }
        public double PrivateAccountPercent { get; set; }
        public double WomanAccountPercent { get; set; }
        public double ManAccountPercent { get; set; }

        public double AccountWith0To15AgePercent { get; set; }
        public double AccountWith15To20AgePercent { get; set; }
        public double AccountWith20To25AgePercent { get; set; }
        public double AccountWith25To30AgePercent { get; set; }
        public double AccountWith30To35AgePercent { get; set; }
        public double AccountWith35To40AgePercent { get; set; }
        public double AccountWith40To120AgePercent { get; set; }
        public double RatherNotSayAccountPercent { get; set; }
        public double CustomAccountPercent { get; set; }

    }







}
