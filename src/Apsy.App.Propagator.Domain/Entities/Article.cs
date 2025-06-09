

namespace Apsy.App.Propagator.Domain.Entities;

public class Article : UserKindDef<User>
{
    public bool IsPromote { get; set; }
    //public int PromoteOrAdsPriceScore { get; set; }
    public DateTime? LatestPromoteDate { get; set; }
    public bool IsCompletedPayment { get; set; }

    public string Author { get; set; }

    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool isCreatedInGroup { get; set; }
    public ArticleType ArticleType { get; set; }
    public DeletedBy DeletedBy { get; set; }
    public bool IsPin { get; set; }
    public DateTime PinDate { get; set; }
    public bool IsVerifield { get; set; }


    public bool IsByAdmin { get; set; }

    public int Hits { get; set; }
    public int ThisWeekHits { get; set; }

    public int SaveArticlesCount { get; set; }
    public int ThisWeekSaveArticlesCount { get; set; }

    public int ArticleCommentsCount { get; set; }
    public int ThisWeekArticleCommentsCount { get; set; }

    public int ArticleLikesCount { get; set; }
    public int ThisWeekArticleLikesCount { get; set; }

    public int NotInterestedArticlesCount { get; set; }
    public int ThisWeekNotInterestedArticlesCount { get; set; }

    public int ShareCount { get; set; }
    public int ThisWeekShareCount { get; set; }

    /// <summary>
    /// the last time the "thisWeeks" were updated
    /// </summary>
    public DateTime LatestUpdateThisWeeks { get; set; }

    public bool IsEdited { get; set; }

    public List<ArticleItem> ArticleItems { get; set; }
    public string ArticleItemsString { get; set; }
    public List<Ads> Adses { get; set; }

    public ICollection<Story> Stories { get; set; }
    public ICollection<SaveArticle> SaveArticles { get; set; }
    public ICollection<ArticleLike> ArticleLikes { get; set; }
    public ICollection<ArticleComment> ArticleComments { get; set; }
    public ICollection<UserSearchArticle> UserSearchArticles { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<NotInterestedArticle> NotInterestedArticles { get; set; }
    public ICollection<UserViewArticle> UserViewArticles { get; set; }
    public ICollection<InterestedUser> InterestedUsers { get; set; }
    public List<Report> Reports { get; set; }
    public List<UserVisitLink> UserVisitLinks { get; set; }
    public List<Link> Links { get; set; }

    public List<Strike> Strikes { get; set; }

    public ICollection<Notification> Notifications { get; set; }

    public List<WarningBanner> WarningBanners { get; set; }



    [GraphQLIgnore]
    public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, CrudType crudType)
    {
        if (crudType == CrudType.DeleteArticle)
        {
            var articleDeletedEvent = new ArticleDeletedEvent()
            {
                AdminId = currrentUser.Id,
                ArticleId = Id,
                ArticleOwnerId = UserId,
                ArticleOwnerEmail = User.Email,
                ArticleItemsString = ArticleItemsString,
                SubTitle = SubTitle,
                Title = Title
            };
            events.Add(articleDeletedEvent);
        }
        else if (crudType == CrudType.VerifyArticle)
        {
            var articleVerifiedEvent = new ArticleVerifiedEvent()
            {
                AdminId = currrentUser.Id,
                ArticleId = Id,
                ArticleOwnerId = UserId,
                ArticleItemsString = ArticleItemsString,
                SubTitle = SubTitle,
                Title = Title
            };
            events.Add(articleVerifiedEvent);
        }
        return events;
    }
}