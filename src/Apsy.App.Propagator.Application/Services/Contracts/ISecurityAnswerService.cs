namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface ISecurityAnswerService : IServiceBase<SecurityAnswer, SecurityAnswerInput>
{
    Task<ListResponseBase<SecurityAnswer>> AddSecurityAnswer(List<SecurityAnswerInput> input, User currentUser);
    Task<ResponseBase<SecurityAnswer>> UpdateSecurityAnswer(SecurityAnswerInput input);
}