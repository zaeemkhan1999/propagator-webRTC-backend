namespace Apsy.App.Propagator.Application.Repositories;

public interface IArticleRepository : IRepository<Article>
{
    Task<Article> UpdateArticlesEngagement(Article article);
    ArticleDto GetArticle(int id,int UserId);
    Article GetArticle(int id);
    IQueryable<ArticleDto> GetArticles(User currentUser);
    IQueryable<Article> GetTopArticles(DateTime from,int UserId);
    IQueryable<Article> GetFollowersArticles(int UserId);

	IQueryable<Article> GetArticleById(int id);

}