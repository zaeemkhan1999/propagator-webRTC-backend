namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class StoryDto : DtoDef
    {
        public string textPositionX { get; set; } //Updated new field
        public string textPositionY { get; set; } //Updated new field
        public string textStyle { get; set; } //Updated new field
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public string ContentAddress { get; set; }
        public StoryType StoryType { get; set; }
        public string Link { get; set; }
        public string Text { get; set; }

        public bool LikedByCurrentUser { get; set; }
        public bool SeenByCurrentUser { get; set; }

        public HighLight HighLight { get; set; }
        public int? HighLightId { get; set; }

        public Post Post { get; set; }
        public int? PostId { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }
        public int? Duration { get; set; }

        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
        public int StorySeensCount { get; set; }
        public int CommentCount { get; set; }
        public DeletedBy DeletedBy { get; set; }
    }
}