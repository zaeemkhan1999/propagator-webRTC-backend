namespace Apsy.App.Propagator.Application.Services.Contracts
{
    internal interface IIPFSCleintService
    {
        Task<string> AddFileContent(string hash);
    }
}
