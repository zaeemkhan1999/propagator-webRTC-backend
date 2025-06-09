namespace Apsy.App.Propagator.Infrastructure.Repositories.Contracts
{
    public interface IExportConversationRepository : IRepository<ExportedConversation>
    {
         
        Task<ExportedConversation> GetAsync(int id);
        Task<List<Message>> GetMessagesbyConversationId(int conversationId);
    }
}
