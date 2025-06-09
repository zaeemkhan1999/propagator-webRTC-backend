namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISecretMessageRepository : IRepository<SecretMessage>
{
    IQueryable<SecretMessageDto> GetDirectMessages(int userId);
    Task<List<SecretMessage>> SeenMessages(List<int> messagesIds);
    SecretMessage GetSecretMessageById(int messageId, bool checkDeleted);
    IQueryable<SecretMessage> GetSecretMessage(int conversationId, int userId);
    IQueryable<SecretConversation> GetSecretConversation(int conversationId);
    SecretConversation GetSecretConversationByIdAndPublicKey(int conversationId, string publicKey);
    SecretConversation GetSecretConversationByFirstOrSecondPublickKey(string publicKey);
    IQueryable<SecretConversation> GetSecretConversationWithOtherUser(int userId, int otherUserId);
    IQueryable<SecretConversationDto> GetUserMessages(int userId, int currentUserId, string publicKey);
}