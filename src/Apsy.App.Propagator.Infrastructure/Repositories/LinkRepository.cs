namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class LinkRepository : Repository<Link, DataReadContext>, ILinkRepository
{
    public LinkRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataReadContext context;

    public IEnumerable<LinkInput> GetUniqueLinksForPost(List<LinkInput> lstInputs)
    {
        return lstInputs.Where(c => !context.Link.Any(x => x.LinkType == LinkType.Post && x.Text == c.Text && x.Url == c.Url));
    }
}
