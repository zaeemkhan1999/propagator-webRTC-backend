using Aps.CommonBack.Messaging.Repositories.Contracts;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IMessageReadRepository
 : IMessageRepositoryBase<Message, Conversation, User>
{
     Conversation GetConversation(int Id);
    IQueryable<ConversationDto> GetConversations(int UserId);
    IQueryable<ConversationDto> GetConversationGroups(int UserId);
    IQueryable<ConversationDto> GroupList(int conversationId, User currentUser);
    IQueryable<Conversation> GetConversation(int otherUserId, User currentUser);
    IQueryable<Conversation> ConversationGetAll();
    IQueryable<Conversation> GetConversation(int Id,string GroupLink);
    IQueryable<Message> GetMessagesByGroupTopic(int Id);
    IQueryable<Message> GetMessages(int Id,bool checkeddelte=false);
    IQueryable<Message> GetMessages(int Id,int UserId);
    IQueryable<MessageDto> GetMessages(int conversationid, User currentUser);
    IQueryable<Message> GetMessages();
    IQueryable<Message> GetMessages(List<int> messagesIds);
    IQueryable<UserMessageGroup> GetUserMessageGroups(int Id,int Userid);
    IQueryable<UserMessageGroup> GetUserMessageGroupsByConversationAndUserId(int userId,int conversationId);
    IQueryable<UserMessageGroup> GetUserMessageGroupsWithoutIsgroup(int Id);
    IQueryable<UserMessageGroup> GetUserMessageGroups(int Id);
    IQueryable<DiscussionsDto> GetDiscussions(User currentUser);
    IQueryable<FollowUserForGroupDto> GetUsersNotInvitedToGroup(int? conversationId, User currentUser);
    IQueryable<FollowUserForGroupDto> GetMyFollowerNotInvitedToGroup(int? conversationId, User currentUser);
    IQueryable<UserMessageGroup> GetUserMessageGroupsNextAdmin(int Id, UserMessageGroup Member);
    IQueryable<UserFollower> GetUserFollowers(int FollowerId, int FollowedId);
    Conversation GetConversationBySubscriptionPlanId(int subscriptionPlanId);
    IQueryable<Conversation> GetMessageConversations();
    IQueryable<UserMessageGroup> GetMessageGroups();
    Task<Conversation> GetConversationById(int id);
}
