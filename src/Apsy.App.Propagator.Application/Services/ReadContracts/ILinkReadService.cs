namespace Apsy.App.Propagator.Application.Services.ReadContracts;

public interface ILinkReadService : IServiceBase<Link, LinkInput>
{
    Task<ListResponseBase<Link>> Get(string searchTerm, int userId);
}