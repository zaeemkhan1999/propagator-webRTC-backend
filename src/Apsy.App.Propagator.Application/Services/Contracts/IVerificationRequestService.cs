

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IVerificationRequestService : IServiceBase<VerificationRequest, VerificationRequestInput>
{
    Task<ResponseBase<VerificationRequest>> AcceptedVerificationRequest(int requestId);
    Task<ResponseBase<VerificationRequest>> RejectVerificationRequest(int requestId, string reason);
    Task<ResponseBase> RemoveVerificationRequest(int requestId);
}