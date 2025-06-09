
namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISuspendReadRepository : IRepository<Suspend>
{
    User GetUserById(int id);
    IQueryable<AdminTodayLimitation> GetAdminTodayLimitation();
}
