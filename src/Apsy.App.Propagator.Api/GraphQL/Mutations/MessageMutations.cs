using Apsy.App.Propagator.Infrastructure.Redis;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class MessageMutations
{
    [GraphQLName("message_createDirectMessage")]
    public async Task<ResponseBase<Message>> CreateDirectMessage(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
                    [Service(ServiceKind.Default)] IMessageService service, [Service] IRedisCacheService redisCache,
                    MessageInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Direct_MessageKey{authentication.CurrentUser.Id}";
        string cacheKeyMy = $"Message_Key_{authentication.CurrentUser.Id}";
        string cacheKeyReciver = $"Direct_MessageKey{input.ReceiverId}";
        string cacheKeyReciverUser = $"Message_Key_{input.ReceiverId}";

        User currentUser = authentication.CurrentUser;
        input.SenderId = currentUser.Id;
        ResponseBase<Message> response = await service.CreateDirectMessage(input, currentUser.Id, (int)input.ReceiverId);
        await redisCache.PublishUpdateAsync("cache_invalidation",cacheKey);
        await redisCache.PublishUpdateAsync("cache_invalidation", cacheKeyMy);
        await redisCache.PublishUpdateAsync("cache_invalidation", cacheKeyReciver);
        await redisCache.PublishUpdateAsync("cache_invalidation", cacheKeyReciverUser);
        return response;
    }

    [GraphQLName("message_removeConversation")]
    public virtual ResponseStatus RemoveConversation(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service, [Service] IRedisCacheService redisCache,
        int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Direct_MessageKey{authentication.CurrentUser.Id}";
        string cacheKeyMy = $"Message_Key_{authentication.CurrentUser.Id}";
        redisCache.PublishUpdateAsync("cache_invalidation", cacheKey);
         redisCache.PublishUpdateAsync("cache_invalidation", cacheKeyMy);

        User currentUser = authentication.CurrentUser;

        return service.RemoveConversation(conversationId, currentUser.Id, authentication.CurrentUser);
    }

    [GraphQLName("message_removeMessage")]
    public virtual ResponseStatus RemoveMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service, [Service] IRedisCacheService redisCache,
        int messageId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Direct_MessageKey{authentication.CurrentUser.Id}";
         redisCache.RemoveAsync(cacheKey);
        return service.RemoveMessage(messageId, authentication.CurrentUser);
    }

    [GraphQLName("message_updateMessage")]
    public virtual async Task<ResponseBase<Message>> UpdateMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service, [Service] IRedisCacheService redisCache,
        string text,
        int messageId,
        int? groupTopicId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Direct_MessageKey{authentication.CurrentUser.Id}";
        User currentUser = authentication.CurrentUser;
       await redisCache.RemoveAsync(cacheKey);
        return service.UpdateMessage(text, messageId, groupTopicId, currentUser.Id);
    }

    [GraphQLName("message_seenMessages")]
    public async Task<ResponseBase<bool>> SeenMessages(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
                    [Service(ServiceKind.Default)] IMessageService service,
                    List<int> messagesIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        ResponseBase<bool> response = await service.SeenMessages(messagesIds, authentication.CurrentUser);
        return response;
    }

    [GraphQLName("message_deliveredMessages")]
    public async Task<ResponseBase<bool>> DeliveredMessages(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
                    [Service(ServiceKind.Default)] IMessageService service,
                    List<int> messagesIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.DeliveredMessages(messagesIds);
    }

    [GraphQLName("message_addUserToGroup")]
    public virtual async Task<ResponseStatus> AddUserToGroup(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        int conversationId, int[] userIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return await service.AddUserToGroup(currentUser.Id, userIds, conversationId);
    }

    [GraphQLName("message_removeUserFromGroup")]
    public ResponseStatus RemoveUserFromGroup(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
                            [Service(ServiceKind.Default)] IMessageService service,
                            int userId,
                            int conversationId
                            )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.RemoveUserFromGroup(userId, conversationId, authentication.CurrentUser);
    }

    [GraphQLName("message_createConversationGroup")]
    public virtual async Task<ResponseBase<Conversation>> CreateConversationGroupAsync(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
                            [Service(ServiceKind.Default)] IMessageService service,
                            GroupMessageInput input,
                            int[] userIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return await service.CreateConversationGroupAsync(currentUser.Id, input,  authentication.CurrentUser, userIds);
    }

    [GraphQLName("message_createGroupMessage")]
    public virtual async Task<ResponseBase<Message>> CreateGroupMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        MessageInput messageInput)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        messageInput.SenderId = currentUser.Id;
        var response= await service.CreateGroupMessage(messageInput, currentUser.Id, authentication.CurrentUser);
        if(response != null)
        {
            await service.redisCacheUpdate((int)messageInput.ConversationId);
        }
        return response;
    }

    [GraphQLName("message_updateGroup")]
    public ResponseStatus UpdateGroup(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        GroupInput input
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.UpdateGroup(input, authentication.CurrentUser);
    }

    [GraphQLName("message_setUserPublisherStatus")]
    public async ValueTask<ResponseStatus> SetUserPublisherStatus(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        SetUserPublisherStatusInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.SetUserPublisherStatus(input, authentication.CurrentUser);
    }

    #region Group topic

    [GraphQLName("message_addGroupTopic")]
    public async ValueTask<ResponseBase<GroupTopic>> AddGroupTopic(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        int conversationId,
        string title)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.AddGroupTopic(conversationId, title, authentication.CurrentUser);
    }

    [GraphQLName("message_updateGroupTopic")]
    public async ValueTask<ResponseBase<GroupTopic>> UpdateGroupTopic(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        int groupTopicId,
        string title)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.UpdateGroupTopic(groupTopicId, title, authentication.CurrentUser);
    }

    [GraphQLName("message_removeGroupTopic")]
    public async ValueTask<ResponseStatus> RemoveGroupTopic(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service(ServiceKind.Default)] IMessageService service,
        int groupTopicId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.ExportChat(groupTopicId, authentication.CurrentUser.Id);
    }

    
    #endregion
}
