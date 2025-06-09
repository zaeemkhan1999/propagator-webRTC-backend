namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class DashboardQueries
{
    [GraphQLName("dashboard_getAdminDashboardInfo")]
    public async Task<ResponseBase<GetDashboardInfoDto>> GetAdminDashboardInfo(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IDashboardReadService service,
        DateTime startDate,
        DateTime endDate)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetDashboardInfo(startDate, endDate);
    }

    [GraphQLName("dashboard_getPostLikesDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetPostLikesDashboardInfo(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IDashboardReadService service,
        DateTime startDate,
        DateTime endDate,
        int postId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetPostLikesDashboardInfo(startDate, endDate, postId);
    }

    [GraphQLName("dashboard_getPostsCommentsDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetPostsCommentsDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int postId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetPostsCommentsDashboardInfo(startDate, endDate, postId);
    }

    [GraphQLName("dashboard_getPostsViewsDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetPostsViewsDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int postId
       )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetPostsViewsDashboardInfo(startDate, endDate, postId);
    }

    [GraphQLName("dashboard_getPostsSavesDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetPostsSavesDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int postId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetPostsSavesDashboardInfo(startDate, endDate, postId);
    }

    [GraphQLName("dashboard_getArticlesLikesDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetArticlesLikesDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int articleId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetArticlesLikesDashboardInfo(startDate, endDate, articleId);
    }

    [GraphQLName("dashboard_getArticlesCommentsDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetArticlesCommentsDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int articleId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetArticlesCommentsDashboardInfo(startDate, endDate, articleId);
    }

    [GraphQLName("dashboard_getArticlesViewsDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetArticlesViewsDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int articleId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetArticlesViewsDashboardInfo(startDate, endDate, articleId);
    }

    [GraphQLName("dashboard_getArticlesSavesDashboardInfo")]
    public async Task<ResponseBase<DashboardInfo>> GetArticlesSavesDashboardInfo(
       [Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IDashboardReadService service,
       DateTime startDate,
       DateTime endDate,
       int articleId
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await service.GetArticlesSavesDashboardInfo(startDate, endDate, articleId);
    }

    [GraphQLName("dashboard_getAdminActivities")]
    public ListResponseBase<EventModel> GetAdminActivities(
                       [Authentication] Authentication authentication,
                       [Service(ServiceKind.Default)] IEventStoreService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return service.Get();
    }
}