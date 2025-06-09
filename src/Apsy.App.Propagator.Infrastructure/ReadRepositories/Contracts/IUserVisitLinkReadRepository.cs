namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserVisitLinkReadRepository : IRepository<UserVisitLink>
{
    IQueryable<User> GetUser();
    IQueryable<UserVisitLink> GetUserVisitLink();
}