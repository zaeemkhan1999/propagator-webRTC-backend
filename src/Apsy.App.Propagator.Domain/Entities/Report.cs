namespace Apsy.App.Propagator.Domain.Entities
{
    public class Report : EntityDef
    {
        public string Text { get; set; }
        public string Reason { get; set; }
        public string Email { get; set; }

        public ReportType ReportType { get; set; }

        public int ReporterId { get; set; }
        public User Reporter { get; set; }

        public Post Post { get; set; }
        public int? PostId { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }

        public Comment Comment { get; set; }
        public int? CommentId { get; set; }

        public ArticleComment ArticleComment { get; set; }
        public int? ArticleCommentId { get; set; }

        public User Reported { get; set; }
        public int? ReportedId { get; set; }

        public Message Message { get; set; }
        public int? MessageId { get; set; }

    }
}