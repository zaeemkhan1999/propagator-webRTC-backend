namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IMessageReadService : IServiceBase<Message, MessageInput>
    {
        ListResponseBase<MessageDto> GetDirectMessages(int? userId,int conversationid, User currentUser);
        ListResponseBase<MessageDto> GetGroupMessages(User currentUser);
        SingleResponseBase<ConversationDto> GetGroup(int id, User currentUser);
        ListResponseBase<ConversationDto> GetGroups(int? userId, User currentUser);
        SingleResponseBase<Conversation> GetConversation(int conversationId, int userId);
        ListResponseBase<ConversationDto> GetUserMessages(int userId);
        Task<ListResponseBase<UserMessageGroup>> GetGroupMembers(int conversationId, User currentUser);
        ListResponseBase<FollowUserForGroupDto> GetMyFollowerNotInvitedToGroup(int? conversationId, User currentUser);
        SingleResponseBase<Conversation> GetConversationWithOtherUser(int otherUserId, User currentUser);
        ListResponseBase<DiscussionsDto> GetDiscussions(User currentUser);
        ListResponseBase<FollowUserForGroupDto> GetUsersNotInvitedToGroup(int? conversationId, User currentUser);
        ValueTask<ListResponseBase<GroupTopic>> GetGroupTopics(int conversationId, User currentUser);
        ResponseBase<Conversation> GetConversationBySubscriptionPlanId(int subscriptionPlanId);
        public Task<ResponseBase<ExportedConversation>> GetExportedChat(int Id);
        ResponseBase<bool> IsGroupLinkExist(string groupLink);
        ResponseBase<string> GenerateUniqueKeyForGroup();
    }
}
