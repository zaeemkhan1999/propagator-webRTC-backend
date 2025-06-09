namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IWarningBannerRepository : IRepository<WarningBanner>
{
    IQueryable<Article> GetArticle();
    IQueryable<Post> GetPost();
}