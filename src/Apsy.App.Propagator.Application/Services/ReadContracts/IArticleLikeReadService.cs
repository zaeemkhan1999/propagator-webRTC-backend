
namespace Apsy.App.Propagator.Application.Services.Contracts;
public interface IArticleLikeReadService : IServiceBase<ArticleLike, ArticleLikeInput>
{
    ListResponseBase<ArticleLikeDto> GetArticleLikes(User currentUser);
}