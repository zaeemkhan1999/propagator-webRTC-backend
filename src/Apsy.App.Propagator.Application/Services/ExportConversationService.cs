using Apsy.App.Propagator.Domain.Common.Inputs;
using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;
using System.Linq;

namespace Apsy.App.Propagator.Application.Services
{
    public class ExportConversationService : ServiceBase<ExportedConversation, ExportConversationInput>, IExportConversationService
    {

        public ExportConversationService(
    IExportConversationRepository repository)
         : base(repository)
        {
            this.repository = repository;
        }

        private readonly IExportConversationRepository repository;
        

        public async Task<ResponseBase<ExportedConversationDto>> ExportChat(int ConversationId,int UserId)
        {
            var conversation = await repository.GetMessagesbyConversationId(ConversationId);
            ExportedConversation exportedConversation = new ()
            {
                CreatedDate = DateTime.UtcNow,
                ExpirtyDate = DateTime.UtcNow.AddDays(30),
                MessageJson = JsonConvert.SerializeObject(conversation),
                UserID = UserId,
            };
            
           
               var  res = repository.Add(exportedConversation);
                ExportedConversationDto Result = new()
                {
                    ExpirtyDate = res.ExpirtyDate,
                MessageJson = JsonConvert.DeserializeObject<List<Message>>(res.MessageJson),
                UserID=res.UserID,
                
                };
                return ResponseBase<ExportedConversationDto>.Success(Result);
           
           
        }
    }
}
