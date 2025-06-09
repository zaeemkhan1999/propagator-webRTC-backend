using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Infrastructure.ReadRepositories;
using Apsy.App.Propagator.Infrastructure.ReadRepositories.Contracts;
using Apsy.App.Propagator.Infrastructure.Repositories;
using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;

namespace Propagator.Api.Extensions
{
    public static class RepositoryRegisteration
    {
        public static void RegisterRepository(this IServiceCollection services)
    
        {
           
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAdRepository, AdRepository>();
            services.AddScoped<IAppealAdsRepository, AppealAdsRepository>();
            services.AddScoped<IArticleCommentRepository, ArticleCommentRepository>();
            services.AddScoped<IArticleLikeRepository, ArticleLikeRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IBlockUserRepository, BlockUserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IFollowerRepository, FollowerRepository>();
            services.AddScoped<IGroupTopicRepository, GroupTopicRepository>();
            services.AddScoped<IHideStoryRepository, HideStoryRepository>();
            services.AddScoped<IHighLightRepository, HighLightRepository>();
            services.AddScoped<IInterestedUserRepository, InterestedUserRepository>();
            services.AddScoped<ILikeArticleCommentRepository, LikeArticleCommentRepository>();
            services.AddScoped<ILikeCommentRepository, LikeCommentRepository>();
            services.AddScoped<ILinkRepository, LinkRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotInterestedArticleRepository, NotInterestedArticleRepository>();
            services.AddScoped<INotInterestedPostRepository, NotInterestedPostRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPlaceRepository, PlaceRepository>();
            services.AddScoped<IPostLikeRepository, PostLikeRepository>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPublicNotificationRepository, PublicNotificationRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IResetPasswordRequestRepository, ResetPasswordRequestRepository>();
            services.AddScoped<ISaveArticleRepository, SaveArticleRepository>();
            services.AddScoped<ISavePostRepository, SavePostRepository>();
            services.AddScoped<ISecretMessageRepository, SecretMessageRepository>();
            services.AddScoped<ISecurityAnswerRepository, SecurityAnswerRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<IStoryCommentRepository, StoryCommentRepository>();
            services.AddScoped<IStoryLikeRepository, StoryLikeRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IStorySeenRepository, StorySeenRepository>();
            services.AddScoped<IStrikeRepository, StrikeRepository>();
            services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            services.AddScoped<ISupportRepository, SupportRepository>();
            services.AddScoped<ISuspendRepository, SuspendRepository>();
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserDiscountRepository, UserDiscountRepository>();
            services.AddScoped<IUserLoginRepository, UserLoginRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSearchAccountRepository, UserSearchAccountRepository>();
            services.AddScoped<IUserSearchArticleRepository, UserSearchArticleRepository>();
            services.AddScoped<IUserSearchGroupRepository, UserSearchGroupRepository>();
            services.AddScoped<IUserSearchPlaceRepository, UserSearchPlaceRepository>();
            services.AddScoped<IUserSearchPostRepository, UserSearchPostRepository>();
            services.AddScoped<IUserSearchTagRepository, UserSearchTagRepository>();
            services.AddScoped<IUsersSubscriptionRepository, UsersSubscriptionRepository>();
            services.AddScoped<IUserVisitLinkRepository, UserVisitLinkRepository>();
           services.AddScoped<IVerificationRequestRepository, VerificationRequestRepository>();
            services.AddScoped<IViewArticleRepository, ViewArticleRepository>();
            services.AddScoped<IViewPostRepository, ViewPostRepository>();
            services.AddTransient<IWarningBannerRepository, WarningBannerRepository>();
            services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
            services.AddScoped<CompressionApiClient>();
            services.AddScoped<IGroupRequestRepository, GroupRequestRepository>();
            services.AddScoped<IExportConversationRepository, ExportConversationRepository>();

            services.AddScoped<IAdReadRepository, AdReadRepository>();

            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IArticleReadRepository, ArticleReadRepository>();
            services.AddScoped<IAppealAdsReadRepository, AppealAdsReadRepository>();
            services.AddScoped<IArticleCommentReadRepository, ArticleCommentReadRepository>();
            services.AddScoped<IArticleLikeReadRepository, ArticleLikeReadRepository>();
            services.AddScoped<IArticleReadRepository, ArticleReadRepository>();
            services.AddScoped<IBlockUserReadRepository, BlockUserReadRepository>();
            services.AddScoped<ICommentReadRepository, CommentReadRepository>();
            services.AddScoped<IDiscountReadRepository, DiscountReadRepository>();
            services.AddScoped<IFollowerReadRepository, FollowerReadRepository>();
            services.AddScoped<IGroupTopicReadRepository, GroupTopicReadRepository>();
            services.AddScoped<IHideStoryReadRepository, HideStoryReadRepository>();
            services.AddScoped<IHighLightReadRepository, HighLightReadRepository>();
            services.AddScoped<ILikeArticleCommentReadRepository, LikeArticleCommentReadRepository>();
            services.AddScoped<ILikeCommentReadRepository, LikeCommentReadRepository>();
            services.AddScoped<ILinkReadRepository, LinkReadRepository>();
            services.AddScoped<IMessageReadRepository, MessageReadRepository>();
            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();
            services.AddScoped<INotInterestedArticleReadRepository, NotInterestedArticleReadRepository>();
            services.AddScoped<INotInterestedPostReadRepository, NotInterestedPostReadRepository>();
            services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();
            services.AddScoped<IPlaceReadRepository, PlaceReadRepository>();
            services.AddScoped<IPostLikeReadRepository, PostLikeReadRepository>();
            services.AddScoped<IPostReadRepository, PostReadRepository>();
            services.AddScoped<IPublicNotificationReadRepository, PublicNotificationReadRepository>();
            services.AddScoped<IReportReadRepository, ReportReadRepository>();
            services.AddScoped<IResetPasswordRequestReadRepository, ResetPasswordRequestReadRepository>();
            services.AddScoped<ISaveArticleReadRepository, SaveArticleReadRepository>();
            services.AddScoped<ISavePostReadRepository, SavePostReadRepository>();
            services.AddScoped<ISecretMessageReadRepository, SecretMessageReadRepository>();
            services.AddScoped<ISecurityAnswerReadRepository, SecurityAnswerReadRepository>();
            services.AddScoped<ISettingsReadRepository, SettingsReadRepository>();
            services.AddScoped<IStoryCommentReadRepository, StoryCommentReadRepository>();
            services.AddScoped<IStoryLikeReadRepository, StoryLikeReadRepository>();
            services.AddScoped<IStoryReadRepository, StoryReadRepository>();
            services.AddScoped<IStorySeenReadRepository, StorySeenReadRepository>();
            services.AddScoped<IStrikeReadRepository, StrikeReadRepository>();
            services.AddScoped<ISupportReadRepository, SupportReadRepository>();
            services.AddScoped<ISuspendReadRepository, SuspendReadRepository>();
            services.AddScoped<IEventStoreReadRepository, EventStoreReadRepository>();
            services.AddScoped<ITagReadRepository, TagReadRepository>();
            services.AddScoped<IUserDiscountReadRepository, UserDiscountReadRepository>();
            services.AddScoped<IUserLoginReadRepository, UserLoginReadRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserSearchAccountReadRepository, UserSearchAccountReadRepository>();
            services.AddScoped<IUserSearchArticleReadRepository, UserSearchArticleReadRepository>();
            services.AddScoped<IUserSearchGroupReadRepository, UserSearchGroupReadRepository>();
            services.AddScoped<IUserSearchPlaceReadRepository, UserSearchPlaceReadRepository>();
            services.AddScoped<IUserSearchPostReadRepository, UserSearchPostReadRepository>();
            services.AddScoped<IUserSearchTagReadRepository, UserSearchTagReadRepository>();
            services.AddScoped<IUsersSubscriptionReadRepository, UsersSubscriptionReadRepository>();
            services.AddScoped<IUserVisitLinkReadRepository, UserVisitLinkReadRepository>();
            services.AddScoped<IVerificationRequestReadRepository, VerificationRequestReadRepository>();
            services.AddScoped<IViewArticleReadRepository, ViewArticleReadRepository>();
            services.AddScoped<IViewPostReadRepository, ViewPostReadRepository>();
            services.AddTransient<IWarningBannerReadRepository, WarningBannerReadRepository>();
            services.AddScoped<IApplicationLogReadRepository, ApplicationLogReadRepository>();
            services.AddScoped<IGroupRequestReadRepository, GroupRequestReadRepository>();
            services.AddScoped<IExportConversationReadRepository, ExportConversationReadRepository>();
            services.AddScoped<ISubscriptionPlanReadRepository, SubscriptionReadPlanRepository>();
            services.AddScoped<IStoryReadRepository, StoryReadRepository>();
            
            
        }
    }
}
