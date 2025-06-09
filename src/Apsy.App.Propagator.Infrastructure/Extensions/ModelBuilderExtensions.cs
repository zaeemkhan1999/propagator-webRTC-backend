using Microsoft.EntityFrameworkCore;

namespace Apsy.App.Propagator.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static void RegisterAllEntities<IType>(this ModelBuilder builder)
    {
        //var types = Assembly.GetExecutingAssembly().GetTypes().Where(s => s.IsClass && !s.IsAbstract && s.IsPublic && s.IsSubclassOf(typeof(IType)));
            
        //foreach (var type in types)
        //{
        //    // On Model Creating
        //    builder.Entity(type);
        //}
        builder.Entity<AdminTodayLimitation>();
        builder.Entity<Ads>();
        builder.Entity<AppealAds>();
        builder.Entity<ApplicationLogs>();
        builder.Entity<AppUser>();
        builder.Entity<Article>();
        builder.Entity<ArticleComment>();
        builder.Entity<ArticleLike>();
        builder.Entity<BlockUser>();
        builder.Entity<Comment>();
        builder.Entity<Conversation>();
        builder.Entity<Discount>();
        builder.Entity<GroupTopic>();
        builder.Entity<HideStory>();
        builder.Entity<HighLight>();
        builder.Entity<InterestedUser>();
        builder.Entity<LikeArticleComment>();
        builder.Entity<LikeComment>();
        builder.Entity<Link>();
        builder.Entity<Message>();
        builder.Entity<Notification>();
        builder.Entity<NotInterestedArticle>();
        builder.Entity<NotInterestedPost>();
        builder.Entity<Payment>();
        builder.Entity<Place>();
        builder.Entity<Post>();
        
        builder.Entity<PostLikes>();
        builder.Entity<PostTag>();
        builder.Entity<PublicNotification>();
        builder.Entity<Report>();
        builder.Entity<ResetPasswordRequest>();
        builder.Entity<SaveArticle>();
        builder.Entity<SavePost>();
        builder.Entity<SecretConversation>();
        builder.Entity<SecretMessage>();
        builder.Entity<SecurityAnswer>();
        builder.Entity<SecurityQuestion>();
        builder.Entity<Settings>();
        builder.Entity<Story>();
        builder.Entity<StoryComment>();
        builder.Entity<StoryLike>();
        builder.Entity<StorySeen>();
        builder.Entity<Strike>();
        builder.Entity<SubscriptionPlan>();
        builder.Entity<Support>();
        builder.Entity<Suspend>();
        builder.Entity<Tag>();
        builder.Entity<User>();
        builder.Entity<UserDiscount>();
        builder.Entity<UserFollower>();
        builder.Entity<UserLogin>();
        builder.Entity<UserMessageGroup>();
        builder.Entity<UserRefreshTokens>();
        builder.Entity<UserSearchAccount>();
        builder.Entity<UserSearchArticle>();
        builder.Entity<UserSearchGroup>();
        builder.Entity<UserSearchPlace>();
        builder.Entity<UserSearchPost>();
        builder.Entity<UserSearchTag>();
        builder.Entity<UsersSubscription>();
        builder.Entity<UserViewArticle>();
        builder.Entity<UserViewPost>();
        builder.Entity<UserViewTag>();
        builder.Entity<UserVisitLink>();
        builder.Entity<VerificationRequest>();
        builder.Entity<ViewArticle>();
        builder.Entity<WarningBanner>();
        builder.Entity<RateLimit>();
    }

    public static void SoftDeleteGolobalFilter(this ModelBuilder modelBuilder)
    {
        Expression<Func<EntityDef, bool>> filterExpr = bm => !bm.IsDeleted;
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            if (mutableEntityType.ClrType.IsAssignableTo(typeof(EntityDef)))
            {
                var parameter = Expression.Parameter(mutableEntityType.ClrType);
                var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
                var lambdaExpression = Expression.Lambda(body, parameter);

                // set filter
                mutableEntityType.SetQueryFilter(lambdaExpression);
            }
        }
    }
}
