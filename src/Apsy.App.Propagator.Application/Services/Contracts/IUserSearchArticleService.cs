namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserSearchArticleService : IServiceBase<UserSearchArticle, UserSearchArticleInput>
{

    ResponseBase<UserSearchArticle> DeleteSearchedArticle(int userId, int articleId,User currentUser);
}
