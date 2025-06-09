

namespace Apsy.App.Propagator.Application.Services.Contracts;
public interface IDashboardReadService : IServiceBase<Post, PostInput>
{
    Task<ResponseBase<GetDashboardInfoDto>> GetDashboardInfo(DateTime startDate, DateTime endDate);
    Task<ResponseBase<DashboardInfo>> GetPostLikesDashboardInfo(DateTime startDate, DateTime endDate, int postId);
    Task<ResponseBase<DashboardInfo>> GetPostsCommentsDashboardInfo(DateTime startDate, DateTime endDate, int postId);
    Task<ResponseBase<DashboardInfo>> GetPostsViewsDashboardInfo(DateTime startDate, DateTime endDate, int postId);
    Task<ResponseBase<DashboardInfo>> GetPostsSavesDashboardInfo(DateTime startDate, DateTime endDate, int postId);
    Task<ResponseBase<DashboardInfo>> GetArticlesLikesDashboardInfo(DateTime startDate, DateTime endDate, int articleId);
    Task<ResponseBase<DashboardInfo>> GetArticlesCommentsDashboardInfo(DateTime startDate, DateTime endDate, int articleId);
    Task<ResponseBase<DashboardInfo>> GetArticlesViewsDashboardInfo(DateTime startDate, DateTime endDate, int articleId);
    Task<ResponseBase<DashboardInfo>> GetArticlesSavesDashboardInfo(DateTime startDate, DateTime endDate, int articleId);
}