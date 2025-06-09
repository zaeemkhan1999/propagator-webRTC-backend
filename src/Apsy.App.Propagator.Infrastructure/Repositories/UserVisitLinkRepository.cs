

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserVisitLinkRepository : Repository<UserVisitLink, DataReadContext>, IUserVisitLinkRepository
{
    public UserVisitLinkRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;

    public IQueryable<User> GetUser()
    {
        var query = context.User.AsQueryable();
        return query;
    }
    public IQueryable<UserVisitLink> GetUserVisitLink()
    {
        var query = context.UserVisitLink.AsQueryable();
        return query;
    }
}