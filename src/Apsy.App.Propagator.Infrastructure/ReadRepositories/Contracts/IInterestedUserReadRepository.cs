namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IInterestedUserReadRepository : IRepository<InterestedUser>
{
    IQueryable<InterestedUser> GetInterestedUsers(int Id);
}
