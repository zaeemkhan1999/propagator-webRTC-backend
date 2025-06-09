using Apsy.App.Propagator.Domain.Common.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IExportConversationReadService : IServiceBase<ExportedConversation, ExportConversationInput>
    {
        public Task<ResponseBase<ExportedConversationDto>> GetExportedChat(int Id);
    }
}
