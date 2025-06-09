namespace Apsy.App.Propagator.Application.Repositories;

public interface IInterestedUserRepository : IRepository<InterestedUser>
{
    IQueryable<InterestedUser> GetInterestedUsers(int Id);
}
