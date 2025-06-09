namespace Apsy.App.Propagator.Application.Services;

public class SupportService : ServiceBase<Support, SupportInput>, ISupportService
{
    public SupportService(ISupportRepository repository) : base(repository)
    {
        this.repository = repository;

    }
    private readonly ISupportRepository repository;
}
