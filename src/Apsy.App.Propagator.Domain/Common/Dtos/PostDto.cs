namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class PostDto : DtoDef
    {
        public bool needAds { get; set; }
        public Post Post { get; set; }
        public DateTime WatchDate { get; set; }
        public bool IsViewed { get; set; }
        public bool IsSaved { get; set; }
        public bool IsNotInterested { get; set; }
        [GraphQLIgnore]
        public bool IsInterested { get; set; }
        public bool IsLiked { get; set; }
        public bool IsYourPost { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int NotInterestedPostsCount { get; set; }
        //public List<PostItem> PostItems { get; set; }
        public string PostItemsString { get; set; }
        public bool IsCompletedPayment { get; set; }

        [GraphQLIgnore]
        public bool IsVideo { get; set; }

        public List<CommentDto> PostComments { get; set; }
        public int PosterFollowerCount { get; set; }

        

    }
}
