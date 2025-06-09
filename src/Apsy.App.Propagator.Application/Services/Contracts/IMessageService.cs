

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IMessageService : IServiceBase<Message, MessageInput>
{
    Task<bool> redisCacheUpdate(int conversationId);
    Task<ResponseBase<Message>> CreateDirectMessage(MessageInput messageInput, int senderId, int receiverId);
    ResponseBase<Message> UpdateMessage(string text, int messageId, int? groupTopicId, int senderId);
    Task<ResponseBase<bool>> SeenMessages(List<int> messagesIds, User currentUser);
    ResponseStatus RemoveConversation(int conversationId, int userId, User currentUser);
    ResponseStatus RemoveMessage(int messageId, User currentUser);
    Task<ResponseBase<Conversation>> CreateConversationGroupAsync(int userId, GroupMessageInput input, User user, int[] userIds = null);
    Task<ResponseStatus> AddUserToGroup(int userId, int[] userIds, int conversationId, bool addToSubscription = true);
    Task<ResponseBase<Message>> CreateGroupMessage(MessageInput messageInput, int senderId, User currentUser);
    ResponseStatus RemoveUserFromGroup(int userId, int conversationId, User currentUser);
    ResponseBase<Conversation> UpdateGroup(GroupInput input, User currentUser);
    Task<ResponseBase<bool>> DeliveredMessages(List<int> messagesIds);
    ValueTask<ResponseStatus> SetUserPublisherStatus(SetUserPublisherStatusInput input, User currentUser);
    ValueTask<ResponseBase<GroupTopic>> AddGroupTopic(int conversationId, string title, User currentUser);
    ValueTask<ResponseBase<GroupTopic>> UpdateGroupTopic(int groupTopicId, string title, User currentUser);
    ValueTask<ResponseStatus> RemoveGroupTopic(int groupTopicId, User currentUser);
    public Task<ResponseBase<ExportedConversation>> ExportChat(int ConversationId, int UserId);
}