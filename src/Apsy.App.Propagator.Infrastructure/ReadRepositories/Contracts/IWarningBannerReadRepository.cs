namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IWarningBannerReadRepository : IRepository<WarningBanner>
{
    IQueryable<Article> GetArticle();
    IQueryable<Post> GetPost();
}