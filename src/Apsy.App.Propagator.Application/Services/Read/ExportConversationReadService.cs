using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common.Inputs;
using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class ExportConversationReadService : ServiceBase<ExportedConversation, ExportConversationInput>, IExportConversationReadService
    {
        private readonly IExportConversationReadRepository repository;
        public ExportConversationReadService(
   IExportConversationReadRepository repository)
        : base(repository)
        {
            this.repository = repository;
        }
        public async Task<ResponseBase<ExportedConversationDto>> GetExportedChat(int Id)
        {
            var res = await repository.GetAsync(Id);
            ExportedConversationDto Result = new()
            {
                ExpirtyDate = res.ExpirtyDate,
                MessageJson = JsonConvert.DeserializeObject<List<Message>>(res.MessageJson),
                UserID = res.UserID,
            };
            return ResponseBase<ExportedConversationDto>.Success(Result);
        }

    }
}
