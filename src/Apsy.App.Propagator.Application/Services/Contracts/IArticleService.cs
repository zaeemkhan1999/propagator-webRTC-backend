

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IArticleService : IServiceBase<Article, ArticleInput>
{
    ResponseBase<ArticleAdsDto> PromoteArticle(PromoteArticleInput input, User currentUser);
    ResponseBase<Article> PinArticle(int articleId, bool pin, User currentUser);
    Task<ResponseBase<Article>> VerifyArticle(int articleId, bool verify, User currentUser);
    Task<ResponseBase<ArticleLike>> LikeArticle(int userId, int articleId, bool isLiked, User currentUser);
    Task<ResponseBase> UnLikeArticle(int userId, int articleId);
    Task<ResponseBase<UserViewArticle>> AddView(int articleId, User currentUser);
    Task<ResponseBase> SaveArticle(int userId, int articleId);
    ResponseBase<Article> Update(ArticleInput input, User currentUser);
    Task<ResponseBase> UnSavePost(int userId, int articleId);
    Task<ResponseStatus> UndoDeleteArticle(int entityId);

}

