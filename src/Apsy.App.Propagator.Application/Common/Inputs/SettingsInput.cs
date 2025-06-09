namespace Apsy.App.Propagator.Application.Common.Inputs;

public class SettingsInput : InputDef
{
    public int? Id { get; set; }
    public int TotalArticlesCount { get; set; }
    public int TotalCommentsCount { get; set; }
    public int TotalTagsCount { get; set; }
    public int TotalPostLikesCount { get; set; }
    public int TotalArticleLikesCount { get; set; }
    public int TotalPromotedPostsCount { get; set; }
    public int TotalAdsesCount { get; set; }
}