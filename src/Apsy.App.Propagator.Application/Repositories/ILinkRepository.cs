namespace Apsy.App.Propagator.Application.Repositories;

public interface ILinkRepository : IRepository<Link>
{
    IEnumerable<LinkInput> GetUniqueLinksForPost(List<LinkInput> lstInputs);
}
