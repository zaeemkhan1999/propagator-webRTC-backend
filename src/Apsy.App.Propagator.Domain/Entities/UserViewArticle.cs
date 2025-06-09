namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserViewArticle : UserKindDef<User>
    {
        public Article Article { get; set; }
        public int ArticleId { get; set; }
    }
}
