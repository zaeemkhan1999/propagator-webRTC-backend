namespace Apsy.App.Propagator.Domain.Entities;

public class UserSearchArticle : UserKindDef<User>
{
    public Article Article { get; set; }
    public int ArticleId { get; set; }
}
