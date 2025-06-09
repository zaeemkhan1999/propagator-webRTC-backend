namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IStrikeRepository : IRepository<Strike>
{ 
    Post GetPostById(int id);
    Article GetArticleById(int id);
    User GetUserById(int id);
    IQueryable<User> GetUser();
    IQueryable<AdminTodayLimitation> GetAdminTodayLimitation();
    IQueryable<Strike> GetStrikes();
    IQueryable<Suspend> GetSuspends();
}