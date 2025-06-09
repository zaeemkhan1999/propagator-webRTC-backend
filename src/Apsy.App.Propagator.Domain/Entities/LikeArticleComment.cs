namespace Apsy.App.Propagator.Domain.Entities
{
    public class LikeArticleComment : UserKindDef<User>
    {
        public ArticleComment ArticleComment { get; set; }
        public int ArticleCommentId { get; set; }
        public ICollection<Notification> Notifications { get; set; }

    }
}
