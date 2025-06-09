

namespace Apsy.App.Propagator.Domain.Entities;

public class ArticleItem : ValueObject.ValueObject
{
    public string Data { get; set; }
    public int Order { get; set; }
    public string VideoTime { get; set; }
    public string VideoShape { get; set; }
    public ArticleItemType ArticleItemType { get; set; }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return Data;
        yield return Order;
        yield return ArticleItemType;
        yield return VideoShape;
    }


    //public Article Article { get; set; }

    //public int ArticleId { get; set; }
}
