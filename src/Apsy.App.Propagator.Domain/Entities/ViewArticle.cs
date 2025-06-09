namespace Apsy.App.Propagator.Domain.Entities;

public class ViewArticle : UserKindDef<User>
{
    public Article Article { get; set; }
    public int ArticleId { get; set; }
}