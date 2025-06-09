using Apsy.App.Propagator.Application.Services.Read;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Infrastructure.Redis;
using Propagator.Common.Services;

namespace Propagator.Api.Extensions
{
    public static class ServicesRegisteration
    {
        public static void RegisterServices(this IServiceCollection services)
        {

            services.AddScoped<IProductReadService, ProductReadService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOpenSearchService,OpenSearchService>();
            services.AddScoped<IUserReadService, UserReadService>();
            services.AddScoped<IPaymentReadService, PaymentReadService>();
            services.AddScoped<IUserVisitLinkReadService, UserVisitLinkReadService>();
            services.AddScoped<IUsersSubscriptionReadService, UsersSubscriptionReadService>();
            services.AddScoped<IUserSearchGroupReadService, UserSearchGroupReadService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStrikeReadService, StrikeReadService>();
            services.AddScoped<IStorySeenReadService, StorySeenReadService>();
            services.AddScoped<IStoryReadService, StoryReadService>();
            services.AddScoped<ISecurityAnswerReadService, SecurityAnswerReadService>();
            services.AddScoped<ISecretMessageReadService, SecretMessageReadService>();
            services.AddScoped<IPublicNotificationReadService, PublicNotificationReadService>();
            services.AddScoped<INotificationReadService, NotificationReadService>();
            services.AddScoped<IMessageReadService, MessageReadService>();
            services.AddScoped<IGroupTopicReadService, GroupTopicReadService>();
            services.AddScoped<IFollowerReadService, FollowerReadService>();
            services.AddScoped<IExportConversationReadService, ExportConversationReadService>();
            services.AddScoped<IDiscountReadService,DiscountReadService>();
            services.AddScoped<ICommentReadService, CommentReadService>();
            services.AddScoped<IArticalReadService, ArticleReadService>();
            services.AddScoped<IArticleCommentReadService, ArticleCommentReadService>();
            services.AddScoped<IAdsReadService, AdsReadService>();
            services.AddScoped<IPostReadService, PostReadService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IJwtManagerService, JwtManagerService>();
            services.AddScoped<IAdsService, AdsService>();
            services.AddScoped<IArticleCommentService, ArticleCommentService>();
            services.AddScoped<IArticleLikeReadService, ArticleLikeReadService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IBlockUserService, BlockUserService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDashboardReadService, DashboardReadService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IFollowerService, FollowerService>();
            services.AddScoped<IHideStoryReadService, HideStoryReadService>();
            services.AddScoped<IHighLightService, HighLightService>();
            services.AddScoped<IInterestedUserService, InterestedUserService>();
            services.AddScoped<ILikeArticleCommentService, LikeArticleCommentService>();
            services.AddScoped<ILikeCommentService, LikeCommentService>();
            services.AddScoped<IPostLikeReadService, PostLikeReadService>();
            services.AddScoped<ILinkReadService, LinkReadService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();    
            services.AddScoped<INotInterestedArticleService, NotInterestedArticleService>();
            services.AddScoped<INotInterestedPostService, NotInterestedPostService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<IPublicNotificationService, PublicNotificationService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ISaveArticleService, SaveArticleService>();
            services.AddScoped<ISavePostService, SavePostService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<ISecretMessageService, SecretMessageService>();
            services.AddScoped<ISecurityAnswerService, SecurityAnswerService>();
            services.AddScoped<IStoryCommentService, StoryCommentService>();
            services.AddScoped<IStoryLikeService, StoryLikeService>();
            services.AddScoped<IStorySeenService, StorySeenService>();
            services.AddScoped<IStoryService, StoryService>();
            services.AddScoped<IStrikeService, StrikeService>();
            services.AddScoped<ISubscriptionPlanReadService, SubscriptionPlanReadService>();
            services.AddScoped<ISupportService, SupportService>();
            services.AddScoped<ISuspendService, SuspendService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserSearchAccountService, UserSearchAccountService>();
            services.AddScoped<IUserSearchArticleService, UserSearchArticleService>();
            services.AddScoped<IUserSearchGroupService, UserSearchGroupService>();
            services.AddScoped<IUserSearchPlaceService, UserSearchPlaceService>();
            services.AddScoped<IUserSearchPostService, UserSearchPostService>();
            services.AddScoped<IUserSearchTagService, UserSearchTagService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUsersSubscriptionService, UsersSubscriptionService>();
            services.AddScoped<IUserVisitLinkService, UserVisitLinkService>();
            services.AddScoped<IVerificationRequestService, VerificationRequestService>();
            services.AddScoped<IViewArticleService, ViewArticleService>();
            services.AddScoped<IViewPostService, ViewPostService>();
            services.AddScoped<IWarningBannerService, WarningBannerService>();
            services.AddScoped<IGroupRequestReadService, GroupRequestReadService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IExportConversationService, ExportConversationService>();
        }
    }
}
