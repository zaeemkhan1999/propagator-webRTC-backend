namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IInterestedUserRepository : IRepository<InterestedUser>
{
    IQueryable<InterestedUser> GetInterestedUsers(int Id);
}
