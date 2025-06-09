using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IArticalReadService: IServiceBase<Article, ArticleInput>
    {
        SingleResponseBase<ArticleDto> GetArticle(int id, int UserId);
        ListResponseBase<ArticleDto> GetArticles(User currentUser);
        ListResponseBase<ArticleDto> GetTopArticles(DateTime from, int UserId);
        ListResponseBase<ArticleDto> GetFollowersArticles(int UserId);
        ListResponseBase<UserViewArticle> GetViews();
        ListResponseBase<SaveArticleDto> GetSavedArticles(User currentUser);
    }
}
