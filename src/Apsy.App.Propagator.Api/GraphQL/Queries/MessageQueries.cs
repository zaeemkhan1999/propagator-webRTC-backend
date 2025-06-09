using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common;
using Apsy.App.Propagator.Infrastructure.Redis;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class MessageQueries
{
    [GraphQLName("message_getMessage")]
    public SingleResponseBase<Message> GetMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("message_getDirectMessages")]
    public async Task<ListResponseBase<MessageDto>> GetDirectMessages(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service, [Service] IRedisCacheService redisCache,
        int? userId, int conversationid)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Direct_MessageKey{authentication.CurrentUser.Id}";
        //var cacheMessage = await redisCache.GetAsync<List<MessageDto>>(cacheKey);
        //if (cacheMessage!=null)
        //{
        //    return ListResponseBase<MessageDto>.Success(cacheMessage.AsQueryable());
        //}
        var dbMessage= service.GetDirectMessages(userId,conversationid, authentication.CurrentUser);
        if (dbMessage !=null)
        {
            await redisCache.SetAsync(cacheKey, dbMessage.Result.ToList(), TimeSpan.FromMinutes(10));
        }
        return dbMessage;
    }

    [GraphQLName("message_getGroupMessages")]
    public async Task< ListResponseBase<MessageDto>> GetGroupMessages(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service, [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"group_Messages{authentication.CurrentUser.Id}";
        var cacheMessages =await redisCache.GetAsync<List<MessageDto>>(cacheKey);
        if (cacheMessages!=null)
        {
            return ListResponseBase<MessageDto>.Success(cacheMessages.AsQueryable());
        }
        var dbMessages= service.GetGroupMessages(authentication.CurrentUser);
        if (dbMessages!=null)
        {
            await redisCache.SetAsync(cacheKey, dbMessages, TimeSpan.FromMinutes(10));
        }
        return dbMessages;
    }

    [GraphQLName("message_getDiscussions")]
    public async Task< ListResponseBase<DiscussionsDto>> GetDiscussions(
       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
       [Service(ServiceKind.Default)] IMessageReadService service, [Service] IRedisCacheService redisCache)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"discussions{authentication.CurrentUser.Id}";
        var cacheDiscussions=await redisCache.GetAsync<List<DiscussionsDto>>(cacheKey);
        if (cacheDiscussions !=null)
        {
            return ListResponseBase<DiscussionsDto>.Success(cacheDiscussions.AsQueryable());
        }
        var dbDiscussions= service.GetDiscussions(authentication.CurrentUser);
        if (dbDiscussions!=null)
        {
            await redisCache.SetAsync(cacheKey, dbDiscussions.Result.ToList(), TimeSpan.FromMinutes(10));
        }
        return dbDiscussions;

    }

    [GraphQLName("message_isGroupLinkExist")]
    public ResponseBase<bool> IsGroupLinkExist(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service,
        string groupLink)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.IsGroupLinkExist(groupLink);
    }

    [GraphQLName("message_generateUniqueKeyForGroup")]
    public ResponseBase<string> GenerateUniqueKeyForGroup(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GenerateUniqueKeyForGroup();
    }

    [GraphQLName("message_getGroup")]
    public SingleResponseBase<ConversationDto> GetGroup(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service,
        int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetGroup(conversationId,authentication.CurrentUser);
    }

    [GraphQLName("message_getGroups")]
    public ListResponseBase<ConversationDto> GetGroups(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService service,
        int? userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetGroups(userId, authentication.CurrentUser);
    }

    [GraphQLName("message_getConversation")]
    public virtual SingleResponseBase<Conversation> GetConversations(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService messageService,
        int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return messageService.GetConversation(conversationId, currentUser.Id);
    }

    [GraphQLName("message_getUserMessages")]
    public async Task <ListResponseBase<ConversationDto>> GetUserMessages(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IMessageReadService messageService,
        [Service] IRedisCacheService redisCache,
        int? userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        string cacheKey = $"Message_Key_{authentication.CurrentUser.Id}";
        var cacheMessage=await redisCache.GetAsync<List<ConversationDto>>(cacheKey);
        if (cacheMessage != null)
        {
            return ListResponseBase<ConversationDto>.Success(cacheMessage.AsQueryable());
        }
       var db_Messages= messageService.GetUserMessages(authentication.CurrentUser.Id);
        if (db_Messages != null)
        {
           await redisCache.SetAsync(cacheKey, db_Messages.Result.ToList(),TimeSpan.FromMinutes(10));
        }
        return db_Messages;
        
    }

    [GraphQLName("message_getGroupMembers")]
    public async Task<ListResponseBase<UserMessageGroup>> GetGroupMembers(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] IMessageReadService messageService,
                                int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await messageService.GetGroupMembers(conversationId, authentication.CurrentUser);
    }

    [GraphQLName("message_getConversationWithOtherUser")]
    public SingleResponseBase<Conversation> GetConversationWithOtherUser(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] IMessageReadService service,
            int otherUserId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.GetConversationWithOtherUser(otherUserId, currentUser);
    }

    [GraphQLName("message_getMyFollowerNotInvitedToGroup")]
    public ListResponseBase<FollowUserForGroupDto> GetMyFollowerNotInvitedToGroup(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] IMessageReadService messageService,
                            int? conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return messageService.GetMyFollowerNotInvitedToGroup(conversationId, authentication.CurrentUser);
    }

    [GraphQLName("message_getUsersNotInvitedToGroup")]
    public ListResponseBase<FollowUserForGroupDto> GetUsersNotInvitedToGroup(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IMessageReadService messageService,
        int? conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return messageService.GetUsersNotInvitedToGroup(conversationId, authentication.CurrentUser);
    }
    #region Group topic

    [GraphQLName("message_getGroupTopics")]
    public async ValueTask<ListResponseBase<GroupTopic>> GetGroupTopics(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] RequestInterception.Authentication authentication,
        [Service] IMessageReadService messageService,
        int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await messageService.GetGroupTopics(conversationId, authentication.CurrentUser);
    }

    #endregion
}