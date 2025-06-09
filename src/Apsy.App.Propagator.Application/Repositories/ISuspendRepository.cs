
namespace Apsy.App.Propagator.Application.Repositories;

public interface ISuspendRepository : IRepository<Suspend>
{
    User GetUserById(int id);
    IQueryable<AdminTodayLimitation> GetAdminTodayLimitation();
}
