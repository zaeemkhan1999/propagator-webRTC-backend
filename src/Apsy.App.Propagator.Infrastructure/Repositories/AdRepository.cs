namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class AdRepository
 : Repository<Ads, DataReadContext>, IAdRepository
{
    public AdRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }


    #region props

    private DataReadContext context;
    #endregion
    public IQueryable<AdsDto> GetbyId(int Id, int UserId)
    {
        return context.Ads.Where(x => x.Id == Id).Select(MapList(UserId));
    }

    public Ads GetbyAdsId(int Id)
    {
        return context.Ads.Where(x => x.Id == Id)
            .Include(c => c.Post)
            .ThenInclude(c => c.Poster)
            .FirstOrDefault();
    }
    public IQueryable<AdsDto> GetbyAll(int UserId)
    {
        return context.Ads.Select(MapList(UserId));
    }

    #region functions
    private Expression<Func<Ads, AdsDto>> MapList(int Id)
    {

        return (ads) => new AdsDto()
        {
            Ads = ads,
            AdsDtoStatus = ads.AdsRejectionStatus == AdsRejectionStatus.Active ? ads.TotalViewed < ads.NumberOfPeopleCanSee ? AdsDtoStatus.Active : AdsDtoStatus.Complete :
                         ads.AdsRejectionStatus == AdsRejectionStatus.Rejected ? AdsDtoStatus.Rejected : AdsDtoStatus.Suspended,
            PostItemsString = ads.Post.PostItemsString,
            IsLiked =  ads.Post.Likes.Any(c => c.UserId == Id && c.Liked),
            IsViewed = ads.Post.UserViewPosts.Any(c => c.UserId == Id),
            IsNotInterested = ads.Post.NotInterestedPosts.Any(c => c.UserId == Id),
            IsSaved = ads.Post.SavePosts.Any(c => c.UserId == Id),
            IsYours = ads.Post.PosterId == Id,
            CommentCount = ads.Post.Comments.Count,
            ShareCount = ads.Post.Messages.Count,
            LikeCount = ads.Post.LikesCount,
            ViewCount = ads.TotalViewed,
            IsAppeal = ads.AppealAdss.Any(d => d.AppealStatus == AppealStatus.Pending),
            AppealAds = ads.AppealAdss
        };

    }

    public IQueryable<AdsDto> GetAdsesForSlider(List<int> ignoredAdsIds, User currentuser)
    {
        var adsQueryAble = context.Ads
           .Where(c =>
              !ignoredAdsIds.Contains(c.Id) &&
              c.AdsRejectionStatus == AdsRejectionStatus.Active &&
              c.IsCompletedPayment &&
              c.IsActive &&
              c.Type == AdsType.Ads &&
              c.TotalViewed < c.NumberOfPeopleCanSee);

        var adses = adsQueryAble
              .Where(c =>
                   c.ManualStatus != ManualStatus.Manual
                          ||
                     c.TargetStartAge <= currentuser.DateOfBirth.GetAge() &&
                     c.TargetEndAge >= currentuser.DateOfBirth.GetAge() &&
                     c.TargetGenders == currentuser.Gender &&
                     c.TargetLocation.Contains(currentuser.Location)
               )
              .Where(c => !c.User.Blocks.Any(c => c.BlockedId == currentuser.Id))
             .Select(MapList(currentuser.Id));
        return adses;
    }


    public IQueryable<Ads> GetAdById(int id)
    {
        var ad = context.Ads.Where(p => p.Id == id).AsNoTracking().AsQueryable();
        return ad;
    }

    public IQueryable<Ads> GetAdsByPostId(int postId)
    {
        var ad = context.Ads.Where(p => p.PostId == postId).AsNoTracking().AsQueryable();
        return ad;
    }
    #endregion
}