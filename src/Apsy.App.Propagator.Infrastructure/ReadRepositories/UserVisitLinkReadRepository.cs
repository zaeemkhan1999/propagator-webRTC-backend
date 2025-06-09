

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserVisitLinkReadRepository : Repository<UserVisitLink, DataWriteContext>, IUserVisitLinkReadRepository
{
    public UserVisitLinkReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;

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