namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IArticleRepository : IRepository<Article>
{
    IQueryable<Article> GetAllArticle();
    Task<Article> UpdateArticlesEngagement(Article article);
    ArticleDto GetArticle(int id, int UserId);
    Article GetArticle(int id);
    IQueryable<ArticleDto> GetArticles(User currentUser);
    IQueryable<Article> GetTopArticles(DateTime from, int UserId);
    IQueryable<Article> GetFollowersArticles(int UserId);

    IQueryable<Article> GetArticleById(int id);
    IQueryable<ArticleDto> GetNewestArticles(User currentUser);
    IQueryable<ArticleDto> GetNewsArticles(User currentUser);
    IQueryable<ArticleDto> MyArticles(User currentUser);
    IQueryable<ArticleDto> MostEngaged(User currentUser);
    IQueryable<ArticleDto> ForYou(User currentUser);
    IQueryable<ArticleDto> Explore(User currentUser);
    bool Ratelimiter(int UserId, string Type);

}