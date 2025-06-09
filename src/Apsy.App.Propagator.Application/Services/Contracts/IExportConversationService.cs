using Apsy.App.Propagator.Domain.Common.Inputs;

namespace Apsy.App.Propagator.Application.Services.Contracts
{
    public interface IExportConversationService :IServiceBase<ExportedConversation, ExportConversationInput>
    {

       public Task<ResponseBase<ExportedConversationDto>> ExportChat(int ConversationId,int UserId);
    }
}
