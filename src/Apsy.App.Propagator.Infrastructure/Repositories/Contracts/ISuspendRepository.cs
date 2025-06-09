
namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISuspendRepository : IRepository<Suspend>
{
    User GetUserById(int id);
    IQueryable<AdminTodayLimitation> GetAdminTodayLimitation();
}
