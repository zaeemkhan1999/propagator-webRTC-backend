namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface INotInterestedArticleService : IServiceBase<NotInterestedArticle, NotInterestedArticleInput>
{
    Task<ResponseBase<NotInterestedArticle>> AddNotInterestedArticle(NotInterestedArticleInput input);
    Task<ResponseBase<NotInterestedArticle>> RemoveFromNotInterestedArticle(NotInterestedArticleInput input);
}