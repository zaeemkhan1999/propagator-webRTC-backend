using Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ILinkReadRepository : IRepository<Link>
{
    IEnumerable<LinkInput> GetUniqueLinksForPost(List<LinkInput> lstInputs);
}
