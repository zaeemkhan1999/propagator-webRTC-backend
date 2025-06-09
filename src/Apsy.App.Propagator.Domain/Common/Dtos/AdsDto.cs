namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class AdsDto : DtoDef
    {
        public Ads Ads { get; set; }
        public AdsDtoStatus AdsDtoStatus { get; set; }


        public bool IsViewed { get; set; }
        public bool IsSaved { get; set; }
        public bool IsNotInterested { get; set; }
        public bool IsLiked { get; set; }
        public bool IsYours { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        //public List<PostItem> PostItems { get; set; }
        public string PostItemsString { get; set; }
        public bool IsAppeal { get; set; }
        public ICollection<AppealAds> AppealAds { get; set; }
    }
}