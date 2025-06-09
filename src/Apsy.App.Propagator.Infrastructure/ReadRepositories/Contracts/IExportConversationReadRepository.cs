namespace Apsy.App.Propagator.Infrastructure.Repositories.Contracts
{
    public interface IExportConversationReadRepository : IRepository<ExportedConversation>
    {
         
        Task<ExportedConversation> GetAsync(int id);
        Task<List<Message>> GetMessagesbyConversationId(int conversationId);
    }
}
