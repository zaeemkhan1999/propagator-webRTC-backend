

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class StrikeReadRepository : Repository<Strike, DataReadContext>, IStrikeReadRepository
{
    public StrikeReadRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;


    public Post GetPostById(int id)
    {
        var query = context.Post.Where(x => x.Id == id).FirstOrDefault();

        return query;
    }
    public Article GetArticleById(int id)
    {
        var query = context.Article.Where(x => x.Id == id).FirstOrDefault();

        return query;
    }
    public User GetUserById(int id)
    {
        var query = context.User.Where(x => x.Id == id).FirstOrDefault();

        return query;
    }

    public IQueryable<User> GetUser()
    {
        var query = context.User.Include(x=>x.Strikes).AsQueryable();
        return query;
    }
    public IQueryable<AdminTodayLimitation> GetAdminTodayLimitation()
    {
        var query = context.AdminTodayLimitation.AsQueryable();
        return query;
    }
    public IQueryable<Strike> GetStrikes()
    {
        var query = context.Strike.AsQueryable();
        return query;
    }
    public IQueryable<Suspend> GetSuspends()
    {
        var query = context.Suspend.AsQueryable();
        return query;
    }
}