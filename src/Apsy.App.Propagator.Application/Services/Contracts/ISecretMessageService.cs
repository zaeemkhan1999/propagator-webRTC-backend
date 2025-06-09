

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface ISecretMessageService : IServiceBase<SecretMessage, SecretMessageInput>
{
    ResponseBase<SecretConversation> CreateConversation(SecretConversationInput input);
    ResponseBase<SecretConversation> JoinToSecretChat(string publicKey, int secretConversationId, User currentUser);
    Task<ResponseBase<SecretMessage>> CreateDirectMessage(SecretMessageInput messageInput, int senderId, int receiverId, User currentUser);
    Task<ResponseBase<bool>> SeenMessages(List<int> messagesIds);
    ResponseStatus RemoveConversation(int conversationId, int userId, string publicKey, User currentUser);
    ResponseStatus RemoveMessage(int messageId, User currentUser);
}