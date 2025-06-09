namespace Apsy.App.Propagator.Domain.Entities;

//[NotMapped]
public class PostItem : ValueObject.ValueObject
{

    public int Order { get; set; }
    public string ThumNail { get; set; }
    public string Content { get; set; }
    public string VideoTime { get; set; }
    public PostItemType PostItemType { get; set; }
    public string SummaryVideoLink { get; set; }
    public string VideoShape { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string Bg { get; set; }
    public string AspectRatio { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return Order;
        yield return ThumNail;
        yield return Content;
        yield return PostItemType;
        yield return SummaryVideoLink;
        yield return VideoShape;
        yield return Width;
        yield return Height;
    }

    //public Post Post { get; set; }
    //public int PostId { get; set; }

}