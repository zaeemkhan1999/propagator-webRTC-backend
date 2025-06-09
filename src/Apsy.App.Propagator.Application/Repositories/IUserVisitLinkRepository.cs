namespace Apsy.App.Propagator.Application.Repositories;

public interface IUserVisitLinkRepository : IRepository<UserVisitLink>
{
    IQueryable<User> GetUser();
    IQueryable<UserVisitLink> GetUserVisitLink();
}