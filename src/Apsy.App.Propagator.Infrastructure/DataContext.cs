using Apsy.App.Propagator.Infrastructure.Configurations;
using Apsy.App.Propagator.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Apsy.App.Propagator.Infrastructure;

public class DataContext : IdentityDbContext<AppUser, IdentityRole, string> /*DbContext*/
{
    public DataContext(DbContextOptions options)
        : base(options)
    {
        
    }
    public DbSet<ProductImages> ProductImages { get; set; }
    public DbSet<Product> Product { get; set; }

    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<PostWatchHistory> PostWatchHistory { get; set; }
    public DbSet<Discount> Discount { get; set; }
    public DbSet<AdminTodayLimitation> AdminTodayLimitation { get; set; }
    public DbSet<Ads> Ads { get; set; }
    public DbSet<AppealAds> AppealAds { get; set; }
    public DbSet<ApplicationLogs> ApplicationLogs { get; set; }
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<Article> Article { get; set; }
    public DbSet<ArticleComment> ArticleComment { get; set; }
    public DbSet<ArticleLike> ArticleLike { get; set; }
    public DbSet<BlockUser> BlockUser { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Conversation> Conversation { get; set; }

    public DbSet<GroupTopic> GroupTopic { get; set; }

    public DbSet<GroupRequest> GroupRequest {get; set;}
    public DbSet<HideStory> HideStory { get; set; }
    public DbSet<HighLight> HighLight { get; set; }
    public DbSet<InterestedUser> InterestedUser { get; set; }
    public DbSet<LikeArticleComment> LikeArticleComment { get; set; }
    public DbSet<LikeComment> LikeComment { get; set; }
    public DbSet<Link> Link { get; set; }
    public DbSet<Message> Message { get; set; }
    public DbSet<Notification> Notification { get; set; }
    public DbSet<NotInterestedArticle> NotInterestedArticle { get; set; }
    public DbSet<NotInterestedPost> NotInterestedPost { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Place> Place { get; set; }
    public DbSet<Post> Post { get; set; }

    public DbSet<PostLikes> PostLikes { get; set; }
    public DbSet<PostTag> PostTag { get; set; }
    public DbSet<PublicNotification> PublicNotification { get; set; }
    public DbSet<Report> Report { get; set; }
    public DbSet<ResetPasswordRequest> ResetPasswordRequest { get; set; }
    public DbSet<SaveArticle> SaveArticle { get; set; }
    public DbSet<SavePost> SavePost { get; set; }
    public DbSet<SecretConversation> SecretConversation { get; set; }
    public DbSet<SecretMessage> SecretMessage { get; set; }
    public DbSet<SecurityAnswer> SecurityAnswer { get; set; }
    public DbSet<SecurityQuestion> SecurityQuestion { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Story> Story { get; set; }
    public DbSet<StoryComment> StoryComment { get; set; }
    public DbSet<StoryLike> StoryLike { get; set; }
    public DbSet<StorySeen> StorySeen { get; set; }
    public DbSet<Strike> Strike { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
    public DbSet<Support> Support { get; set; }
    public DbSet<Suspend> Suspend { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserDiscount> UserDiscount { get; set; }
    public DbSet<UserFollower> UserFollower { get; set; }
    public DbSet<UserLogin> UserLogin { get; set; }
    public DbSet<UserMessageGroup> UserMessageGroup { get; set; }
    public DbSet<UserRefreshTokens> UserRefreshToken { get; set; }
    public DbSet<UserSearchAccount> UserSearchAccount { get; set; }
    public DbSet<UserSearchArticle> UserSearchArticle { get; set; }
    public DbSet<UserSearchGroup> UserSearchGroup { get; set; }
    public DbSet<UserSearchPlace> UserSearchPlace { get; set; }
    public DbSet<UserSearchPost> UserSearchPost { get; set; }
    public DbSet<UserSearchTag> UserSearchTag { get; set; }
    public DbSet<UsersSubscription> UsersSubscription { get; set; }
    public DbSet<UserViewArticle> UserViewArticle { get; set; }
    public DbSet<UserViewPost> UserViewPost { get; set; }
    public DbSet<UserViewTag> UserViewTag { get; set; }
    public DbSet<UserVisitLink> UserVisitLink { get; set; }
    public DbSet<VerificationRequest> VerificationRequest { get; set; }
    public DbSet<ViewArticle> ViewArticle { get; set; }
    public DbSet<ExportedConversation> ExportedConversation { get; set; }
    public DbSet<WarningBanner> WarningBanner { get; set; }
    public DbSet<RateLimit> RateLimit { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RegisterAllEntities<EntityDef>();

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        modelBuilder.SoftDeleteGolobalFilter();

        base.OnModelCreating(modelBuilder);
    
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}