//using Aps.CommonBack.Social.Post.Comment.Models.Entities;



namespace Apsy.App.Propagator.Domain.Entities;

public class Post : EntityDef
{
    #region 
    public ICollection<Comment> Comments { get; set; }
    public string Duration { get; set; }
    public int PosterId { get; set; }

    public User Poster { get; set; }

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PostLikes> Likes { get; set; }
    #endregion

    public string Bg { get; set; }
    public string AspectRatio { get; set; }
    public bool IsPromote { get; set; }
    //public int PromoteOrAdsPriceScore { get; set; }
    public DateTime? LatestPromoteDate { get; set; }
    public bool IsCompletedPayment { get; set; }
    public string YourMind { get; set; }
    public DeletedBy DeletedBy { get; set; }
    public bool AllowDownload { get; set; }

    public string Location { get; set; }

    public bool IsPin { get; set; }
    public DateTime PinDate { get; set; }
    public PostType PostType { get; set; }

    public bool IsByAdmin { get; set; }
    public int Hits { get; set; }
    public int ThisWeekHits { get; set; }

    public int SavePostsCount { get; set; }
    public int ThisWeekSavePostsCount { get; set; }

    public int CommentsCount { get; set; }
    public int ThisWeekCommentsCount { get; set; }

    public int LikesCount { get; set; }
    public int ThisWeekLikesCount { get; set; }

    public int NotInterestedPostsCount { get; set; }
    public int ThisWeekNotInterestedPostsCount { get; set; }

    public int ShareCount { get; set; }
    public int ThisWeekShareCount { get; set; }

    /// <summary>
    /// the last time the "thisWeeks" were updated
    /// </summary>
    public DateTime LatestUpdateThisWeeks { get; set; }

    public bool IsEdited { get; set; }
    public bool IsCreatedInGroup { get; set; }
    public IconLayoutType IconLayoutType { get; set; }
    public List<WarningBanner> WarningBanners { get; set; }
    public ICollection<UserViewPost> UserViewPosts { get; set; }
    public ICollection<Story> Stories { get; set; }
    public ICollection<SavePost> SavePosts { get; set; }
    [GraphQLIgnore]
    public List<PostItem> PostItems { get; set; }
    /// <summary>
    /// this is th above PostItems convert to string
    /// </summary>
    public string PostItemsString { get; set; }
    public List<Ads> Adses { get; set; }
    // public Ads Ads { get; set; }
    //public int AdsId { get; set; }

    public ICollection<UserSearchPost> UserSearchPosts { get; set; }
    /// <summary>
    /// list of messages withcn this postShared
    /// </summary>
    public ICollection<Message> Messages { get; set; }
    public ICollection<NotInterestedPost> NotInterestedPosts { get; set; }
    public ICollection<InterestedUser> InterestedUsers { get; set; }

    public List<Report> Reports { get; set; }
    public List<string> Tags { get; set; }
    public string StringTags { get; set; }
    public int? TaskId { get; set; }
    public List<UserVisitLink> UserVisitLinks { get; set; }
    public List<Link> Links { get; set; }

    public ICollection<Notification> Notifications { get; set; }
    public List<Strike> Strikes { get; set; }

    [GraphQLIgnore]
    public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, CrudType crudType)
    {
        if (crudType == CrudType.DeletePost)
        {
            var postDeletedEvent = new PostDeletedEvent()
            {
                AdminId = currrentUser.Id,
                PostOwnerEmail = Poster?.Email,
                PostOwnerId = PosterId,
                YourMind = YourMind,
                PostId = Id,
                PostItemsString = PostItemsString
            };
            events.Add(postDeletedEvent);
        }

        return events;
    }
}