using Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ILinkRepository : IRepository<Link>
{
    IEnumerable<LinkInput> GetUniqueLinksForPost(List<LinkInput> lstInputs);
}
