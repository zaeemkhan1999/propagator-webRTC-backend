namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserVisitLinkRepository : IRepository<UserVisitLink>
{
    IQueryable<User> GetUser();
    IQueryable<UserVisitLink> GetUserVisitLink();
}