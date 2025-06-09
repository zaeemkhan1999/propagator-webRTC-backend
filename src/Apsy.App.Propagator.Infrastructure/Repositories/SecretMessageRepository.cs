using Mapster;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SecretMessageRepository : Repository<SecretMessage, DataReadContext>, ISecretMessageRepository
{
    public SecretMessageRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataReadContext context;
    #region functions
    public IQueryable<SecretMessageDto> GetDirectMessages(int userId)
    {
        var secratemessage = context.SecretMessage.Where(c =>
                      (c.SenderId == userId && !c.IsSenderDeleted) ||
                      (c.ReceiverId == userId && !c.IsReceiverDeleted))
                .ProjectToType<SecretMessageDto>();

        return secratemessage;
    }
    public async Task<List<SecretMessage>> SeenMessages(List<int> messagesIds)
    {
        var messages = await context.SecretMessage.Where(c => messagesIds.Contains(c.Id)).ToListAsync();
        return messages;
    }
    public SecretMessage GetSecretMessageById(int messageId, bool checkDeleted)
    {
        var messages = context.SecretMessage.IgnoreQueryFilters().FirstOrDefault(c => c.Id == messageId);
        return messages;
    }

    public IQueryable<SecretMessage> GetSecretMessage(int conversationId, int userId)
    {
        var messages = context.SecretMessage.Where(c => c.SecretConversationId == conversationId
                                        && (c.SenderId == userId || c.ReceiverId == userId));
        return messages;
    }

    public IQueryable<SecretConversation> GetSecretConversation(int conversationId)
    {
        return context.SecretConversation.Where(x => x.Id == conversationId).AsNoTracking().AsQueryable();
    }

    public SecretConversation GetSecretConversationByFirstOrSecondPublickKey(string publicKey)
    {
        return context.SecretConversation
        .Where(c => c.FirstUserPublicKey == publicKey || c.SecondUserPublicKey == publicKey)
                            .FirstOrDefault();
    }

    public IQueryable<SecretConversation> GetSecretConversationWithOtherUser(int userId, int otherUserId)
    {
        return context.SecretConversation
            .Where(c =>
             (c.FirstUserId == userId && c.SecondUserId == otherUserId)
             ||
             (c.SecondUserId == userId && c.FirstUserId == otherUserId));
    }

    public SecretConversation GetSecretConversationByIdAndPublicKey(int conversationId, string publicKey)
    {
        return context.SecretConversation
                        .Where(a => a.Id == conversationId && (a.FirstUserPublicKey == publicKey || a.SecondUserPublicKey == publicKey))
                        .FirstOrDefault();
    }

    public IQueryable<SecretConversationDto> GetUserMessages(int userId, int currentUserId, string publicKey)
    {
        var baseQuery = context.SecretConversation
               .Where(c => (c.FirstUserPublicKey == publicKey || c.SecondUserPublicKey == publicKey) && (c.FirstUserId == userId || c.SecondUserId == userId));
             var query =  baseQuery.Select(c => new SecretConversationDto
               {
                   ConversationId = c.Id,
                   LatestMessageDate = c.LatestMessageDate,
                   UnreadCount = c.FirstUserId == userId ? c.FirstUnreadCount : c.SecondUnreadCount,
                   LastMessage = c.SecretMessages
                                        .Where(c => (c.SenderId == currentUserId && !c.IsSenderDeleted) || (c.ReceiverId == currentUserId && !c.IsReceiverDeleted))
                                        .OrderBy(x => x.CreatedDate)
                                        .LastOrDefault(),

                   IsFirstUserDeleted = c.IsFirstUserDeleted,
                   FirstUserDeletedDate = c.FirstUserDeletedDate,

                   IsSecondUserDeleted = c.IsSecondUserDeleted,
                   SecondUserDeletedDate = c.SecondUserDeletedDate,
                   LatestMessageUserId = c.LatestMessageUserId,

                   Email = c.FirstUserId == userId ? c.SecondUser.Email : c.FirstUser.Email,
                   DisplayName = c.FirstUserId == userId ? c.SecondUser.DisplayName : c.FirstUser.DisplayName,
                   ImageAddress = c.FirstUserId == userId ? c.SecondUser.ImageAddress : c.FirstUser.ImageAddress,
                   Cover = c.FirstUserId == userId ? c.SecondUser.Cover : c.FirstUser.Cover,
                   UserTypes = c.FirstUserId == userId ? c.SecondUser.UserTypes : c.FirstUser.UserTypes,
                   Username = c.FirstUserId == userId ? c.SecondUser.Username : c.FirstUser.Username,
                   LastSeen = c.FirstUserId == userId ? c.SecondUser.LastSeen : c.FirstUser.LastSeen,
               });
        return query;
    }
    #endregion
}