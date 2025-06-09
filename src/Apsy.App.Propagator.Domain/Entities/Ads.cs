namespace Apsy.App.Propagator.Domain.Entities;

public class Ads : UserKindDef<User>
{

    public VisitType VisitType { get; set; }

    public string TargetLocation { get; set; }

    public int? TargetStartAge { get; set; }

    public int? TargetEndAge { get; set; }

    public Gender? TargetGenders { get; set; }

    public ManualStatus ManualStatus { get; set; }

    public int NumberOfPeopleCanSee { get; set; }

    public double? PricePerPerson { get; set; }

    /// <summary>
    /// total money withc user pay for this ads
    /// </summary>
    public double? TotlaAmount { get; set; }
    /// <summary>
    /// this is all views witch it gained
    /// </summary>
    public int TotalViewed { get; set; }

    public string TicketNumber { get; set; }


    public AdsType Type { get; set; }

    public Post Post { get; set; }
    public int? PostId { get; set; }

    public Article Article { get; set; }
    public int? ArticleId { get; set; }

    public List<Payment> Payments { get; set; }
    public bool IsCompletedPayment { get; set; }

    /// <summary>
    /// is it latest and active Ads
    /// and  tolaviewd < total View  
    /// </summary>
    /// </summary>
    public bool IsActive { get; set; }


    public string LatestPaymentIntentId { get; set; }
    public DateTime LatestPaymentDateTime { get; set; }


    public AdsRejectionStatus AdsRejectionStatus { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<AppealAds> AppealAdss { get; set; }

    public new int? UserId { get; set; }

    // public Payment Payment { get; set; }
    //public int PaymentId { get; set; }
    //views , likes ,...  1500 ta
    public string DiscountCode { get; set; }


    [GraphQLIgnore]
    public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, CrudType crudType)
    {
        if (crudType == CrudType.RejectAds)
        {
            var adsRejectedEvent = new AdsRejectedEvent()
            {
                AdminId = currrentUser.Id,
                PostOwnerEmail = Post?.Poster?.Email,
                PostOwnerId = Post?.PosterId,
                AdsRejectionStatus = AdsRejectionStatus,
                AdsType = Type,
                YourMind = Post?.YourMind,
                AdsId = Id,
                PostId=PostId,
                ArticleId=ArticleId,
                PostItemsString=Post?.PostItemsString,
                ArticleItemsString=Article?.ArticleItemsString
            };
            events.Add(adsRejectedEvent);
        }
        else if (crudType == CrudType.UnRejectAds)
        {
            var adsUnRejectedEvent = new AdsUnRejectedEvent()
            {
                AdminId = currrentUser.Id,
                PostOwnerEmail = Post?.Poster?.Email,
                PostOwnerId = Post?.PosterId,
                AdsRejectionStatus = AdsRejectionStatus,
                AdsType = Type,
                YourMind = Post?.YourMind,
                AdsId = Id,
                PostId = PostId,
                ArticleId = ArticleId,
                PostItemsString = Post?.PostItemsString,
                ArticleItemsString = Article?.ArticleItemsString
            };
            events.Add(adsUnRejectedEvent);
        }
        else if (crudType == CrudType.SuspendAds)
        {
            var adsSuspendedEvent = new AdsSuspendedEvent()
            {
                AdminId = currrentUser.Id,
                PostOwnerEmail = Post?.Poster?.Email,
                PostOwnerId = Post?.PosterId,
                AdsRejectionStatus = AdsRejectionStatus,
                AdsType = Type,
                YourMind = Post?.YourMind,
                AdsId = Id,
                PostId = PostId,
                ArticleId = ArticleId,
                PostItemsString = Post?.PostItemsString,
                ArticleItemsString = Article?.ArticleItemsString

            };
            events.Add(adsSuspendedEvent);
        }
        else if (crudType == CrudType.UnSuspendAds)
        {
            var adsUnSuspendEvent = new AdsUnSuspendEvent()
            {
                AdminId = currrentUser.Id,
                PostOwnerEmail = Post?.Poster?.Email,
                PostOwnerId = Post?.PosterId,
                AdsRejectionStatus = AdsRejectionStatus,
                AdsType = Type,
                YourMind = Post?.YourMind,
                AdsId = Id,
                PostId = PostId,
                ArticleId = ArticleId,
                PostItemsString = Post?.PostItemsString,
                ArticleItemsString = Article?.ArticleItemsString
            };
            events.Add(adsUnSuspendEvent);
        }
        else if (crudType == CrudType.AdsWithOutPayment)
        {
            var adsWithOutPaymentAddedEvent = new AdsWithOutPaymentAddedEvent()
            {
                AdminId = currrentUser.Id,
                PostOwnerEmail = Post?.Poster?.Email,
                PostOwnerId = Post?.PosterId,
                AdsRejectionStatus = AdsRejectionStatus,
                AdsType = Type,
                YourMind = Post?.YourMind,
                AdsId = Id,
                PostId = PostId,
                ArticleId = ArticleId,
                PostItemsString = Post?.PostItemsString,
                ArticleItemsString = Article?.ArticleItemsString
            };
            events.Add(adsWithOutPaymentAddedEvent);
        }
        return events;
    }

}