using Aps.CommonBack.Base.Repositories.Contracts;

namespace Apsy.App.Propagator.Application.Repositories;

public interface IWarningBannerRepository : IRepository<WarningBanner>
{
    IQueryable<Article> GetArticle();
    IQueryable<Post> GetPost();
}