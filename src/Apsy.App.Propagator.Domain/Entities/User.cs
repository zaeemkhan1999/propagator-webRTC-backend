namespace Apsy.App.Propagator.Domain.Entities;
public class User : UserDef /*SocialUserDef<UserFollower>*/
{
    [GraphQLIgnore]
    public AppUser AppUser { get; set; }

    //[ForeignKey(nameof(AppUser))]
    //public string AppUserId { get; set; }

    public bool IsDeletedAccount { get; set; }
    public DateTime DeleteAccountDate { get; set; }

    public string LinkBio { get; set; }

    /// <summary>
    /// Refer to account id of customer in stripe
    /// </summary>
    public string StripeAccountId { get; set; }

    /// <summary>
    /// Refer to equivalent user id in stripe
    /// </summary>
    public string StripeCustomerId { get; set; }

    /// <summary>
    /// Refer to subscription of user in stripe for pay for SubscriptionPlan
    /// </summary>
    public string SubscriptionId { get; set; }

    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
    public DateTime? SuspensionLiftingDate { get; set; }

    public UserTypes UserTypes { get; set; }

    public string Bio { get; set; }

    public string DisplayName { get; set; }

    public string LegalName { get; set; }

    public string Username { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string PhoneNumber { get; set; }
    public string CountryCode { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool IsVerified { get; set; }

    public string ImageAddress { get; set; }
    public string Cover { get; set; }

    public Gender Gender { get; set; }

    public string Location { get; set; }    
    public string Ip { get; set; }

    public bool isOnline { get; set; }

    public bool isTyping { get; set; }
    public bool DirectNotification { get; set; }

    public bool FolloweBacknotification { get; set; }

    public bool LikeNotification { get; set; }

    public bool CommentNotification { get; set; }

    //public bool EnableTwoFactorAuthentication { get; set; }

    public bool PrivateAccount { get; set; }

    public bool ProfessionalAccount { get; set; }
    public DateTime LastSeen { get; set; }
    public bool HasStory { get; set; }

    //[GraphQLIgnore]
    //public string TwoFactorCode { get; set; }

    [GraphQLIgnore]
    public ICollection<StoryComment> StoryComments { get; set; }
    [GraphQLIgnore]
    public ICollection<StoryLike> StoryLikes { get; set; }
    [GraphQLIgnore]
    public ICollection<Story> Stories { get; set; }
    [GraphQLIgnore]
    public ICollection<StorySeen> StorySeens { get; set; }
    [GraphQLIgnore]
    public ICollection<Article> Articles { get; set; }
    public ICollection<SaveArticle> SaveArticles { get; set; }
    [GraphQLIgnore]
    public ICollection<ArticleLike> ArticleLikes { get; set; }
    [GraphQLIgnore]
    public ICollection<ArticleComment> ArticleComments { get; set; }
    [GraphQLIgnore]
    public ICollection<Post> Posts { get; set; }
    [GraphQLIgnore]
    public ICollection<PostLikes> PostLikes { get; set; }
    public ICollection<UserDiscount> UserDiscounts { get; set; }

    public ICollection<Comment> CommentPosts { get; set; }
    public ICollection<SavePost> SavePosts { get; set; }
    public ICollection<NotInterestedPost> NotInterestedPosts { get; set; }
    public ICollection<InterestedUser> InterestedUsers { get; set; }
    public ICollection<EventModel> EventModels { get; set; }
    public ICollection<AppealAds> AppealAdss { get; set; }

    //public ICollection<InterestedUser> FollowerUsers { get; set; }
    /// <summary>
    /// this is the id of the posts (not notInterestedIds)
    /// </summary>
    public List<int> NotInterestedPostIds { get; set; }

    [GraphQLIgnore]
    public ICollection<LikeComment> LikeComments { get; set; }
    public ICollection<Comment> CommentMentions { get; set; }
    public ICollection<ArticleComment> ArticleCommentMentions { get; set; }
    /// <summary>
    /// i hide them
    /// </summary>
    [GraphQLIgnore]
    public ICollection<HideStory> HidedStory { get; set; }
    /// <summary>
    /// they hide me
    /// </summary>
    [GraphQLIgnore]
    public ICollection<HideStory> HiderStory { get; set; }
    /// <summary>
    /// Gets or sets they block me .
    /// </summary>
    public ICollection<BlockUser> Blockers { get; set; }
    /// <summary>
    /// Gets or sets i block them .
    /// </summary>
    public ICollection<BlockUser> Blocks { get; set; }
    [GraphQLIgnore]
    public ICollection<Report> Reporters { get; set; }
    [GraphQLIgnore]
    public ICollection<Report> Reports { get; set; }
    [GraphQLIgnore]
    public ICollection<Support> Supports { get; set; }
    [GraphQLIgnore]
    public ICollection<UserSearchPost> UserSearchPosts { get; set; }
    public ICollection<UserSearchArticle> UserSearchArticles { get; set; }
    [GraphQLIgnore]
    public ICollection<UserSearchGroup> UserSearchGroups { get; set; }
    [GraphQLIgnore]
    public ICollection<UserSearchTag> UserSearchTags { get; set; }
    [GraphQLIgnore]
    public ICollection<UserSearchPlace> UserSearchPlaces { get; set; }
    [GraphQLIgnore]
    public ICollection<UserSearchAccount> SearcherAccounts { get; set; }
    [GraphQLIgnore]
    public ICollection<UserSearchAccount> SearchedAccounts { get; set; }
    [GraphQLIgnore]
    public ICollection<VerificationRequest> VerificationRequests { get; set; }
    [GraphQLIgnore]
    public ICollection<Payment> Payments { get; set; }
    [GraphQLIgnore]
    public ICollection<NotInterestedArticle> NotInterestedArticles { get; set; }
    /// <summary>
    /// this is the id of the articles (not NotInterestedArticles)
    /// </summary>
    public List<int> NotInterestedArticleIds { get; set; }
    [GraphQLIgnore]
    public ICollection<UserMessageGroup> UserMessageGroups { get; set; }
    [GraphQLIgnore]
    public ICollection<UserViewPost> UserViewPosts { get; set; }
    public ICollection<UserViewArticle> UserViewArticles { get; set; }
    [GraphQLIgnore]
    public ICollection<Notification> SenderNotifications { get; set; }
    [GraphQLIgnore]
    public ICollection<Notification> RecieverNotifications { get; set; }
    /// <summary>
    /// people who they follow me
    /// </summary>
    public ICollection<UserFollower> Followers { get; set; }
    /// <summary>
    /// people who i follow them  
    /// </summary>
    public ICollection<UserFollower> Followees { get; set; }
    [GraphQLIgnore]
    public ICollection<Ads> Ads { get; set; }

    [GraphQLIgnore]
    public List<UserViewTag> UserViewTags { get; set; }
    [GraphQLIgnore]
    public List<Strike> Strikes { get; set; }

    [GraphQLIgnore]
    public List<WarningBanner> WarningBanners { get; set; }
    [GraphQLIgnore]
    public List<Suspend> Suspends { get; set; }

    public ICollection<ResetPasswordRequest> ResetPasswordRequests { get; set; }


    [GraphQLIgnore]
    public int GetAge()
    {
        // Save today's date.
        var today = DateTime.Today;

        // Calculate the age.
        var age = today.Year - DateOfBirth.Year;
        return age;
    }

    [GraphQLIgnore]
    public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, bool isBaned, CrudType crudType)
    {
        if (crudType == CrudType.AccountBaned)
        {
            var accountBanedEvent = new AccountBanedEvent()
            {
                AdminId = currrentUser.Id,
                UserId = Id,
                IsBaned = isBaned,
                UserEmail = Email,
                UserImageAddress = ImageAddress,
                UserCover = Cover,
                UserDisplayName = DisplayName,
                UserName = Username

            };
            events.Add(accountBanedEvent);
        }

        if (crudType == CrudType.DeleteAccount)
        {
            var accountDeletedEvent = new AccountDeletedEvent()
            {
                AdminId = currrentUser.Id,
                DeletedUserEmail = Email,
                DeletedUserId = Id,
                DeletedUserImageAddress = ImageAddress,
                DeletedUserCover = Cover,
                DeletedUserPhoneNumber = PhoneNumber,
                DeletedUserUsername = Username,
            };
            events.Add(accountDeletedEvent);
        }
        if (crudType == CrudType.UsersUnSuspendedEvent)
        {
            var usersUnSuspendedEvent = new UsersUnSuspendedEvent()
            {
                AdminId = currrentUser.Id,
                UserId = Id,
                UserEmail = Email,
                UserImageAddress = ImageAddress,
                UserCover = Cover,
                UserDisplayName = DisplayName,
                UserName = Username
            };
            events.Add(usersUnSuspendedEvent);
        }
        return events;
    }

}