using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Apsy.App.Propagator.Infrastructure.Repositories
{
    public class ExportConversationRepository : Repository<ExportedConversation, DataReadContext>, IExportConversationRepository
    {
        public ExportConversationRepository(IDbContextFactory<DataReadContext> dbContextFactory)
   : base(dbContextFactory)
        {
            context = dbContextFactory.CreateDbContext();
        }

        #region props

        private DataReadContext context;
        #endregion
        public async Task<ExportedConversation> GetAsync(int id)
        {
            return await context.ExportedConversation.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Message>> GetMessagesbyConversationId(int conversationId)
        {
            return  await context.Message.Where(x => x.ConversationId == conversationId)
                 .Include(c => c.Receiver)
                 .Include(c => c.Sender)
                .ToListAsync();
        }
    }
}
