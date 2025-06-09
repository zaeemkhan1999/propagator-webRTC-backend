namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class LinkReadRepository : Repository<Link, DataWriteContext>, ILinkReadRepository
{
    public LinkReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    private readonly DataWriteContext context;

    public IEnumerable<LinkInput> GetUniqueLinksForPost(List<LinkInput> lstInputs)
    {
        return lstInputs.Where(c => !context.Link.Any(x => x.LinkType == LinkType.Post && x.Text == c.Text && x.Url == c.Url));
    }
}
