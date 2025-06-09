using Aps.CommonBack.Messaging.Generics.Responses;
using Apsy.App.Propagator.Infrastructure.Redis;
using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;

namespace Apsy.App.Propagator.Application.Services;

public class MessageService : ServiceBase<Message, MessageInput>, IMessageService
{
    public MessageService(
        IRedisCacheService redisCache,
        IMessageRepository repository,
        IHttpContextAccessor httpContextAccessor,
        ITopicEventSender topicEventSender,
        IPublisher publisher,
        IExportConversationRepository conversationRepository,
        IArticleRepository articleRepository,
        IHideStoryRepository hideStoryRepository,
        IBlockUserRepository blockUserRepository,
        IInterestedUserService interestedUserService,
        IGroupTopicRepository groupTopicRepository) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        this.topicEventSender = topicEventSender;
        _publisher = publisher;
        _articleRepository = articleRepository;
        _hideStoryRepository = hideStoryRepository;
        _blockUserRepository = blockUserRepository;
        _exportConversation = conversationRepository;
        this.interestedUserService = interestedUserService;
        _groupTopicRepository = groupTopicRepository;
        _redisCache = redisCache;
    }
    private readonly IRedisCacheService _redisCache;
    private readonly IBlockUserRepository _blockUserRepository;
    private readonly IMessageRepository repository;
    private readonly IHideStoryRepository _hideStoryRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITopicEventSender topicEventSender;
    private readonly IPublisher _publisher;
    private readonly IInterestedUserService interestedUserService;
    private readonly IGroupTopicRepository _groupTopicRepository;
    private readonly IExportConversationRepository _exportConversation;
    public virtual async Task<ResponseBase<Message>> CreateDirectMessage(
                                         MessageInput messageInput,
                                         int senderId,
                                         int receiverId)
    {
        var validation = ValidateCreateMessage(ref messageInput, false);
        if (validation.Status != ResponseStatus.Success)
            return validation;

        Conversation conversation = (!(messageInput.ConversationId > 0)) ?
            repository
                .GetMessageConversations().Where(x => (x.FirstUserId == (int?)senderId || x.FirstUserId == (int?)receiverId) && (x.SecondUserId == (int?)senderId || x.SecondUserId == (int?)receiverId))
                .FirstOrDefault()
            :
            repository
                .GetMessageConversations().Where(a => a.Id == messageInput.ConversationId)
                .FirstOrDefault();

        if (conversation == null)
        {
            conversation = messageInput.Adapt<Conversation>();
            conversation.FirstUserId = senderId;
            conversation.SecondUserId = receiverId;
        }

        if (receiverId == conversation.FirstUserId)
        {
            conversation.FirstUnreadCount++;
        }
        else
        {
            conversation.SecondUnreadCount++;
        }

        conversation.LatestMessageDate = DateTime.UtcNow;
        if (conversation.Id > 0)
        {
            conversation = repository.Update(conversation);
        }

        Message message = messageInput.Adapt<Message>();
        if (conversation.Id > 0)
        {
            message.ConversationId = conversation.Id;
        }
        else
        {
            repository.Add(conversation);
            message.Conversation = conversation;
        }

        message.SenderId = senderId;
        int? reciverId = (conversation.FirstUserId == message.SenderId) ? conversation.SecondUserId : conversation.FirstUserId;
        var receiver = repository.Find<User>((int)reciverId);
        if (receiver == null)
            return ResponseStatus.UserNotFound;

        Message result = repository.Add(message);
        if (message.MessageType == MessageType.Post)
        {
            var post = repository.Find<Post>((int)messageInput.PostId);
            if (post != null && messageInput.IsShare)
            {
                post.ShareCount++;
                repository.Update(post);
                await interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = post.PosterId, UserId = senderId, InterestedUserType = InterestedUserType.Post, PostId = post.Id });

            }
        }

        if (message.MessageType == MessageType.Article)
        {
            var article = repository.Find<Article>((int)messageInput.ArticleId);
            if (article != null && messageInput.IsShare)
            {
                article.ShareCount++;
                repository.Update(article);
                await interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = article.UserId, UserId = senderId, InterestedUserType = InterestedUserType.Article, ArticleId = article.Id });

            }
        }

        if (receiver.DirectNotification)
            await SendDirectMessageSubscriptionAsync(result, senderId, (int)reciverId);
        return ResponseBase<Message>.Success(result);
    }

    private ResponseBase<Message> ValidateCreateMessage(ref MessageInput messageInput, bool isGroup)
    {
        var senderId = messageInput.SenderId;
        var receiverId = messageInput.ReceiverId;
        if (!isGroup)
        {
            if (senderId == receiverId)
            {
                return ResponseBase<Message>.Failure(MessagingResponseStatus.SelfMessageNotAllowed);
            }
            if (senderId <= 0 || receiverId <= 0)
            {
                return ResponseBase<Message>.Failure(ResponseStatus.NotEnoghData);
            }
        }

        if (messageInput.MessageType == MessageType.Profile)
        {
            var result = ProfileValidation(ref messageInput, false);
            if (result.Status != ResponseStatus.Success)
                return result;
        }

        if (messageInput.MessageType == MessageType.Story)
        {
            var result = StoryValidation(ref messageInput, false);
            if (result.Status != ResponseStatus.Success)
                return result;
        }

        if (messageInput.MessageType == MessageType.Post)
        {
            var result = PostValidation(ref messageInput, isGroup);
            if (result.Status != ResponseStatus.Success)
                return result;
        }

        if (messageInput.MessageType == MessageType.Article)
        {
            var result = ArticleValidation(ref messageInput, isGroup);
            if (result.Status != ResponseStatus.Success)
                return result;
        }
        return ResponseStatus.Success;
    }

    private ResponseBase<Message> ProfileValidation(ref MessageInput messageInput, bool isGroup)
    {
        if (messageInput.MessageType != MessageType.Profile)
            return ResponseStatus.NotAllowd;

        if (messageInput.ProfileId == null)
            return CustomResponseStatus.ProfileIdIsRequired;

        var profileId = messageInput.ProfileId;
        var receiverId = messageInput.ReceiverId;
        var senderId = messageInput.SenderId;
        var profileOwner = repository
                            .Where<User>(c => c.Id == profileId)
                            .FirstOrDefault();
        if (profileOwner == null)
            return ResponseStatus.NotFound;

        var isBlocked = _blockUserRepository.IsBlocked(profileOwner.Id, (int)senderId);

        if (isBlocked)
            return CustomResponseStatus.UserAlreadyBlockedByContentOwner;

        if (!isGroup)
        {
            var isBlockedByOwner = _blockUserRepository.IsBlocked(profileOwner.Id, (int)receiverId);

            if (isBlockedByOwner)
                return CustomResponseStatus.UserAlreadyBlockedByContentOwner;
        }

        messageInput.PostId = null;
        messageInput.ArticleId = null;
        messageInput.StoryId = null;
        return ResponseStatus.Success;
    }

    private ResponseBase<Message> StoryValidation(ref MessageInput messageInput, bool isGroup)
    {
        if (messageInput.MessageType != MessageType.Story)
            return ResponseStatus.NotAllowd;

        if (messageInput.StoryId == null)
            return CustomResponseStatus.StoryIdIsRequired;

        var storyId = messageInput.StoryId;
        var receiverId = messageInput.ReceiverId;
        var senderId = messageInput.SenderId;
        var storyOwner = repository
                            .Where<Story>(c => c.Id == storyId)
                            .Select(c => c.User)
                            .FirstOrDefault();
        if (storyOwner == null)
            return ResponseStatus.NotFound;

        var isBlocked = _blockUserRepository.IsBlocked(storyOwner.Id, (int)senderId);

        if (isBlocked)
            return CustomResponseStatus.UserAlreadyBlockedByContentOwner;

        var isSenderFollowedStoryOwner = repository
                .Any<UserFollower>(c => c.FollowerId == senderId
                                    && c.FollowedId == storyOwner.Id);

        if (storyOwner.PrivateAccount && !isSenderFollowedStoryOwner && messageInput.SenderId != storyOwner.Id)
        {
            return CustomResponseStatus.CanNotSendPrivateAccountContentToNonFollower;
        }

        if (!isGroup)
        {
            var isHideStory = _hideStoryRepository.IsHideStory(storyOwner.Id, (int)receiverId).Any();
            if (isHideStory)
                return CustomResponseStatus.UserAlreadyBlockedByContentOwner;
            var isBlockedByOwner = _blockUserRepository.IsBlocked(storyOwner.Id, (int)receiverId);
            if (isBlockedByOwner)
                return CustomResponseStatus.UserAlreadyBlockedByContentOwner;
        }
        messageInput.PostId = null;
        messageInput.ArticleId = null;
        messageInput.ProfileId = null;
        return ResponseStatus.Success;
    }

    private ResponseBase<Message> PostValidation(ref MessageInput messageInput, bool isGroup)
    {
        if (messageInput.MessageType != MessageType.Post)
            return ResponseStatus.NotAllowd;

        var postId = messageInput.PostId;
        var receiverId = messageInput.ReceiverId;
        var senderId = messageInput.SenderId;

        if (messageInput.PostId == null)
            return CustomResponseStatus.PostIdIsRequired;
        var postOwner = repository
                        .Where<Post>(c => c.Id == postId)
                        .Select(c => c.Poster)
                        .FirstOrDefault();
        if (postOwner == null)
            return ResponseStatus.NotFound;

        var isBlocked = _blockUserRepository.IsBlocked(postOwner.Id, (int)senderId);

        if (isBlocked)
            return CustomResponseStatus.UserAlreadyBlockedByContentOwner;

        var isSenderFollowedStoryOwner = repository.GetUserFollowers((int)senderId, postOwner.Id).Any();
        if (postOwner.PrivateAccount && !isSenderFollowedStoryOwner && messageInput.SenderId != postOwner.Id)
        {
            return CustomResponseStatus.CanNotSendPrivateAccountContentToNonFollower;
        }

        if (!isGroup)
        {
            var isBlockedByOwner = _blockUserRepository.IsBlocked(postOwner.Id, (int)receiverId);
            repository.Any<BlockUser>(x => x.BlockerId == postOwner.Id && x.BlockedId == receiverId);
            if (isBlockedByOwner)
                return CustomResponseStatus.UserAlreadyBlockedByContentOwner;
        }

        messageInput.StoryId = null;
        messageInput.ArticleId = null;
        messageInput.ProfileId = null;
        return ResponseStatus.Success;
    }

    private ResponseBase<Message> ArticleValidation(ref MessageInput messageInput, bool isGroup)
    {
        if (messageInput.MessageType != MessageType.Article)
            return ResponseStatus.NotAllowd;

        if (messageInput.ArticleId == null)
            return CustomResponseStatus.ArticleIdIsRequired;

        var articleId = messageInput.ArticleId;
        var receiverId = messageInput.ReceiverId;
        var senderId = messageInput.SenderId;

        var articleOwner = _articleRepository.GetArticle((int)articleId).User;

        if (articleOwner == null)
            return ResponseStatus.NotFound;

        var isBlocked = _blockUserRepository.IsBlocked(articleOwner.Id, (int)senderId);
        if (isBlocked)
            return CustomResponseStatus.UserAlreadyBlockedByContentOwner;

        var isSenderFollowedStoryOwner = repository.GetUserFollowers((int)senderId, articleOwner.Id).Any();

        if (articleOwner.PrivateAccount && !isSenderFollowedStoryOwner && messageInput.SenderId != articleOwner.Id)
        {
            return CustomResponseStatus.CanNotSendPrivateAccountContentToNonFollower;
        }

        if (!isGroup)
        {
            var isBlockedByOwner = _blockUserRepository.IsBlocked(articleOwner.Id, (int)receiverId);

            if (isBlockedByOwner)
                return CustomResponseStatus.UserAlreadyBlockedByContentOwner;
        }

        messageInput.StoryId = null;
        messageInput.PostId = null;
        messageInput.ProfileId = null;
        return ResponseStatus.Success;
    }

    public virtual ResponseBase<Message> UpdateMessage(string text, int messageId, int? groupTopicId, int senderId)
    {
        Message val = repository.GetMessages(messageId).FirstOrDefault();
        if (val == null)
        {
            return ResponseBase<Message>.Failure(ResponseStatus.NotEnoghData);
        }

        if (senderId != val.SenderId)
        {
            return ResponseBase<Message>.Failure(CustomMessagingResponseStatus.OnlySenderCanUpdateTheMessage);
        }

        if (val.MessageType == MessageType.Post ||
            val.MessageType == MessageType.Story ||
            val.MessageType == MessageType.Article ||
            val.MessageType == MessageType.Profile)
        {
            return ResponseBase<Message>.Failure(CustomMessagingResponseStatus.InvalidMessageType);
        }

        if (groupTopicId.HasValue)
        {
            var isGroupTopicExist = _groupTopicRepository.GetGroupTopicById(val.ConversationId, (int)groupTopicId).Any();
            if (isGroupTopicExist)
            {
                return CustomMessagingResponseStatus.GroupTopicNotFound;
            }
        }

        Message val2 = val;
        val2.GroupTopicId = groupTopicId;
        val2.Text = text;
        val2.IsEdited = true;
        val2.Update<Message>(val2);
        Message result = repository.Update<Message>(val2);
        return ResponseBase<Message>.Success(result);
    }

    public ResponseStatus Remove(int entityId, int userId)
    {
        Message singleResponseBase = repository.GetMessages(entityId).FirstOrDefault();
        Conversation byId = repository.GetConversation(singleResponseBase.ConversationId);
        if (singleResponseBase.Conversation.FirstUserId != userId && singleResponseBase.Conversation.SecondUserId != userId)
        {
            return ResponseStatus.NotAllowd;
        }

        return SoftDelete(entityId);
    }

    public async Task<ResponseBase<bool>> SeenMessages(List<int> messagesIds, User currentUser)
    {

        var messages = await repository.GetMessages(messagesIds).ToListAsync();
        foreach (var item in messages)
        {
            item.IsSeen = true;
            item.SeenDate = DateTime.UtcNow;
        }

        try
        {
            await repository.UpdateRangeAsync(messages);
            var conversation = repository.GetConversation(messages.FirstOrDefault().ConversationId);
            if (conversation.FirstUserId == currentUser.Id)
                conversation.FirstUnreadCount = 0;
            else
                conversation.SecondUnreadCount = 0;

            await repository.UpdateAsync<Conversation>(conversation);

            return true;

        }
        catch
        {
            return false;
        }
    }

    public async Task<ResponseBase<bool>> DeliveredMessages(List<int> messagesIds)
    {
        var messages = await repository.GetMessages(messagesIds).ToListAsync();
        foreach (var item in messages)
        {
            item.IsDelivered = true;
        }
        try
        {
            await repository.UpdateRangeAsync(messages);
            return true;
        }
        catch
        {
            return false;
        }

    }

    public ResponseStatus RemoveConversation(int conversationId, int userId, User currentUser)
    {
        Conversation conversation = repository.ConversationGetAll().Where(a => a.Id == conversationId).Include(c => c.Admin).FirstOrDefault();
        if (conversation == null)
        {
            return ResponseStatus.NotFound;
        }

        if (conversation.IsGroup && conversation.AdminId != currentUser.Id)
        {
            return CustomMessagingResponseStatus.DeleteGroupNotAllowed;
        }
        if (!conversation.IsGroup && conversation.FirstUserId != userId && conversation.SecondUserId != userId)
        {
            return ResponseStatus.NotAllowd;
        }

        if (conversation.IsDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        try
        {
            repository.BeginTransaction();

            conversation.IsDeleted = true;
            var messages = repository.GetMessages(conversationId, currentUser.Id).ToList();

            foreach (var message in messages)
            {
                message.IsDeleted = true;
            }

            repository.UpdateRange(messages);
            repository.Update(conversation);

            repository.CommitTransaction();
            return ResponseBase.Success();
        }
        catch (Exception ex)
        {
            repository.RollBackTransaction();
            return new CustomResponseStatus(10000, ex.Message);
        }
    }

    public ResponseStatus RemoveMessage(int messageId, User currentUser)
    {
        Message message = repository.GetMessages(messageId).FirstOrDefault();
        if (message == null)
        {
            return ResponseStatus.NotFound;
        }
        Conversation conversation = repository.GetConversation(message.Id);
        if (message.IsDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        if (!message.Conversation.IsGroup && message.Conversation.FirstUserId != currentUser.Id && message.Conversation.SecondUserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        if (!message.Conversation.IsGroup)
        {
            if (message.SenderId != currentUser.Id)
                return ResponseStatus.NotAllowd;
        }
        else if (message.Conversation.AdminId != currentUser.Id && message.SenderId != currentUser.Id)
        {
            return ResponseStatus.NotAllowd;
        }

        Message val = repository.Remove(message);
        if (val.IsDeleted)
        {
            return ResponseStatus.Success;
        }

        return ResponseStatus.Failed;
    }
    public async Task<ResponseBase<Conversation>> CreateConversationGroupAsync(int userId, GroupMessageInput input, User currentUser, int[] userIds = null)
    {
        if (input.GroupName.HasNoValue())
        {
            return ResponseStatus.RequiredDataNotFilled;
        }
        var Conversations = repository.ConversationGetAll();
        if (input.IsPrivate == false)
        {
            var groupNameExist = Conversations.Any(x => x.GroupName == input.GroupName && x.IsPrivate == false && x.IsDeleted == false);

            if (groupNameExist)
                return CustomMessagingResponseStatus.GroupNameAlreadyExists;

            var linkExist = Conversations.Any(x => x.GroupLink == input.GroupLink && x.IsPrivate == false && x.IsDeleted == false);
            if (linkExist)
                return CustomMessagingResponseStatus.GroupLinkAlreadyExists;
        }
        else
        {
            var groupNameExist = Conversations.Any(x => x.GroupName == input.GroupName && x.AdminId == currentUser.Id && x.IsPrivate == true && x.IsDeleted == false);
            if (groupNameExist)
                return CustomMessagingResponseStatus.GroupNameAlreadyExists;

            var linkExist = Conversations.Any(x => x.GroupLink == input.GroupLink && x.AdminId == currentUser.Id && x.IsPrivate == true && x.IsDeleted == false);
            if (linkExist)
                return CustomMessagingResponseStatus.GroupLinkAlreadyExists;
        }
        Conversation conversation = input.Adapt<Conversation>();
        conversation.IsGroup = true;
        conversation.AdminId = currentUser.Id;

        repository.Add(conversation);

        List<int> userList = userIds.ToList();
        if (conversation.Id > 0)
        {
            repository.Add(new UserMessageGroup
            {
                UserId = userId,
                ConversationId = conversation.Id,
                IsAdmin = true,
                IsPublisher = true
            });
            if (userIds != null)
            {
                await AddUserToGroup(userId, userList.ToArray(), conversation.Id, addToSubscription: false);
            }

            return new ResponseBase<Conversation>(conversation);
        }

        return ResponseStatus.UnknownError;
    }

    public async Task<ResponseStatus> AddUserToGroup(int userId, int[] userIds, int conversationId, bool addToSubscription = true)
    {
        if (conversationId < 1)
        {
            return ResponseStatus.RequiredDataNotFilled;
        }

        IQueryable<UserMessageGroup> usergroup = repository.GetUserMessageGroupsByConversationAndUserId(userId, conversationId);
        if (!usergroup.Any())
        {
            return CustomMessagingResponseStatus.UserNotBelongToGroup;
        }

        Conversation conversation = repository.GetConversation(conversationId);
        if (conversation == null)
            return ResponseStatus.NotFound;

        if (conversation.AdminId != userId)
            return ResponseStatus.NotAllowd;

        IQueryable<UserMessageGroup> usersInGroup = repository.GetUserMessageGroupsWithoutIsgroup(conversationId);
        List<UserMessageGroup> entities = new List<UserMessageGroup>();
        foreach (int id in userIds)
        {
            if (!usersInGroup.Any((UserMessageGroup x) => x.UserId == id) && id != userId)
            {
                entities.Add(new UserMessageGroup
                {
                    ConversationId = conversationId,
                    UserId = id,
                    IsPublisher = !conversation.IsGroup || !conversation.IsPrivate
                });
            }
        }

        repository.AddRange(entities);
        if (addToSubscription)
        {
            await UserAddedToGroupMessageSubscription(conversation, entities);
        }

        return ResponseStatus.Success;
    }

    public virtual async Task<ResponseBase<Message>> CreateGroupMessage(MessageInput messageInput, int senderId, User currentUser)
    {
        var validation = ValidateCreateMessage(ref messageInput, true);
        if (validation.Status != ResponseStatus.Success)
            return validation;
        if (senderId <= 0 || messageInput.ConversationId <= 0)
        {
            return ResponseBase<Message>.Failure(ResponseStatus.RequiredDataNotFilled);
        }

        Conversation conversation = repository.GetConversation((int)messageInput.ConversationId);
        if (conversation == null)
        {
            return ResponseBase<Message>.Failure(CustomMessagingResponseStatus.ConversationNotExist);
        }

        var member = await repository.GetUserMessageGroups((int)messageInput.ConversationId, currentUser.Id).SingleOrDefaultAsync();
        if (member == null)
        {
            return CustomMessagingResponseStatus.UserNotBelongToGroup;
        }

        if (!member.IsPublisher)
        {
            return CustomMessagingResponseStatus.UserHasNotPermissionToPublishContent;
        }

        if (messageInput.GroupTopicId.HasValue)
        {
            var isGroupTopicExist = _groupTopicRepository.GetGroupTopicById((int)messageInput.ConversationId, (int)messageInput.GroupTopicId).Any();

            if (!isGroupTopicExist)
            {
                return CustomMessagingResponseStatus.GroupTopicNotFound;
            }
        }

        conversation.LatestMessageDate = DateTime.UtcNow;
        conversation = repository.Update(conversation);
        Message message = messageInput.Adapt<Message>();
        message.Conversation = conversation;
        message.SenderId = senderId;
        Message result = repository.Add(message);
        var userGroups = repository.GetUserMessageGroups(conversation.Id, senderId).ToList();
        userGroups.ForEach(a =>
        {
            a.UnreadCount++;
            repository.Update(a);
        });
        await GroupMessageAddedSubscription(result, conversation.Id);
        return ResponseBase<Message>.Success(result);
    }

    public ResponseStatus RemoveUserFromGroup(int userId, int conversationId, User currentUser)
    {
        UserMessageGroup member = repository.GetUserMessageGroups(conversationId, currentUser.Id).FirstOrDefault();
        if (member == null)
        {
            return CustomMessagingResponseStatus.UserNotBelongToGroup;
        }
        Conversation conversation = repository.GetConversation(conversationId);
        if (conversation is null)
            return ResponseStatus.NotFound;
        if (conversation.AdminId != currentUser.Id && currentUser.Id != member.UserId)
        {
            return ResponseStatus.NotAllowd;
        }

        if (member.UserId == conversation.AdminId)
        {
            var nextAdmin = repository.GetUserMessageGroupsNextAdmin(conversationId, member).FirstOrDefault();
            if (nextAdmin != null)
                conversation.AdminId = nextAdmin.UserId;
        }
        var isLatestMemberOfGroup = !repository.Any<UserMessageGroup>(x => x.ConversationId == conversationId && x.Id != member.Id);

        try
        {
            repository.BeginTransaction();
            repository.Remove(member);
            if (isLatestMemberOfGroup)
                repository.Remove(conversation);
            else
                repository.Update(conversation);
            repository.CommitTransaction();
            return ResponseStatus.Success;

        }
        catch
        {
            repository.RollBackTransaction();
            return ResponseStatus.Failed;
        }
    }
    public async Task<bool> redisCacheUpdate(int conversationId)
    {
        var group = repository.GetConversation(conversationId);
        var memberId = group.UserGroups.Select(u => u.UserId);
        if (memberId != null)
        {
            foreach (var id in memberId)
            {
                string cacheKey = $"discussions{id}";
                await _redisCache.PublishUpdateAsync("cache_invalidation", cacheKey);
            }
            return true;
        }
        return false;
    }
    public ResponseBase<Conversation> UpdateGroup(GroupInput input, User currentUser)
    {


        Conversation conversation = repository.GetConversation(input.ConversationId);

        //var linkExist = repository.GetConversation(input.ConversationId, input.GroupLink).Any();

        //if (linkExist)
        //    return ResponseStatus.AlreadyExists;

        if (conversation == null)
        {
            return ResponseStatus.NotFound;
        }

        if (conversation.AdminId != currentUser.Id)
        {
            return CustomMessagingResponseStatus.DeleteGroupNotAllowed;
        }

        conversation.IsPrivate = (bool)input.IsPrivate;
        conversation.GroupName = input.GroupName;
        conversation.GroupDescription = input.GroupDescription;
        conversation.GroupImgageUrl = input.GroupImgageUrl;
        conversation.GroupLink = input.GroupLink;
        return repository.Update(conversation);
    }

    public async ValueTask<ResponseStatus> SetUserPublisherStatus(SetUserPublisherStatusInput input, User currentUser)
    {
        var conversation = repository.GetConversation(input.ConversationId);


        if (conversation == null)
        {
            return CustomMessagingResponseStatus.ConversationNotExist;
        }
        if (currentUser.UserTypes == UserTypes.User)
        {
            var isCurrentUserAdmin = repository.GetUserMessageGroups(input.ConversationId, currentUser.Id).Where(x => x.IsAdmin).Any();
            if (!isCurrentUserAdmin)
            {
                return ResponseStatus.NotAllowd;
            }
        }

        var userMember = repository.GetUserMessageGroups(input.ConversationId, currentUser.Id).SingleOrDefault();

        if (userMember == null)
        {
            return CustomMessagingResponseStatus.UserNotBelongToGroup;
        }

        userMember.IsPublisher = input.IsPublisher;
        try
        {
            await repository.UpdateAsync(userMember);

            return ResponseStatus.Success;
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }

    #region Group topic

    public async ValueTask<ResponseBase<GroupTopic>> AddGroupTopic(int conversationId, string title, User currentUser)
    {

        var conversation = repository.GetConversation(conversationId);
        if (conversation == null)
        {
            return CustomMessagingResponseStatus.ConversationNotExist;
        }
        if (currentUser.UserTypes == UserTypes.User)
        {
            var isCurrentUserAdmin = repository.GetUserMessageGroups(conversationId, currentUser.Id).Where(x => x.IsAdmin).Any();

            if (!isCurrentUserAdmin)
            {
                return ResponseStatus.NotAllowd;
            }
        }

        var groupTopic = new GroupTopic
        {
            ConversationId = conversationId,
            CreatedDate = DateTime.UtcNow,
            Title = title
        };

        try
        {
            await _groupTopicRepository.AddAsync(groupTopic);

            return ResponseBase<GroupTopic>.Success(groupTopic);
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }
    public async ValueTask<ResponseBase<GroupTopic>> UpdateGroupTopic(int groupTopicId, string title, User currentUser)
    {
        var groupTopic = _groupTopicRepository.GetGroupTopicById(groupTopicId).FirstOrDefault();

        if (groupTopic == null)
        {
            return ResponseStatus.NotFound;
        }
        if (currentUser.UserTypes == UserTypes.User)
        {
            var isCurrentUserAdmin = repository.GetUserMessageGroups(groupTopic.ConversationId, currentUser.Id).Where(x => x.IsAdmin).Any();

            if (!isCurrentUserAdmin)
            {
                return ResponseStatus.NotAllowd;
            }
        }

        groupTopic.Title = title;

        try
        {
            await _groupTopicRepository.UpdateAsync(groupTopic);

            return ResponseBase<GroupTopic>.Success(groupTopic);
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }
    public async ValueTask<ResponseStatus> RemoveGroupTopic(int groupTopicId, User currentUser)
    {
        var groupTopic = _groupTopicRepository.GetGroupTopicById(groupTopicId).SingleOrDefault();


        if (groupTopic == null)
        {
            return ResponseStatus.NotFound;
        }
        if (currentUser.UserTypes == UserTypes.User)
        {
            var isCurrentUserAdmin = repository.GetUserMessageGroups(groupTopic.ConversationId, currentUser.Id).Where(x => x.IsAdmin).Any();
            if (!isCurrentUserAdmin)
            {
                return ResponseStatus.NotAllowd;
            }
        }

        var messages = repository.GetMessagesByGroupTopic(groupTopicId).ToList();


        groupTopic.IsDeleted = true;

        var transaction = await repository.BeginTransactionAsync();
        try
        {
            await repository.UpdateAsync(groupTopic);

            messages.ForEach(x => x.GroupTopicId = null);
            await repository.UpdateRangeAsync(messages);

            await transaction.CommitAsync();

            return ResponseStatus.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            return ResponseStatus.Failed;
        }
    }
    #endregion

    private async ValueTask SendDirectMessageSubscriptionAsync(Message message, int senderId, int reciverId)
    {
        try
        {
            await _publisher.Publish(new SendDirectMessageEvent(message, senderId, reciverId));
        }
        catch
        {
        }
    }

    private async ValueTask GroupMessageAddedSubscription(Message message, int groupConversationId)
    {
        await topicEventSender.SendAsync($"{groupConversationId}_GroupMessageAdded", message);
    }

    private async ValueTask UserAddedToGroupMessageSubscription(Conversation group, List<UserMessageGroup> users)
    {
        foreach (var item in users)
        {
            await topicEventSender.SendAsync($"{item.UserId}_UserAddedToGroup", group);
        }
    }
    public async Task<ResponseBase<ExportedConversation>> ExportChat(int ConversationId, int UserId)
    {
        var conversation = await _exportConversation.GetMessagesbyConversationId(ConversationId);
        ExportedConversation exportedConversation = new()
        {
            CreatedDate = DateTime.UtcNow,
            ExpirtyDate = DateTime.UtcNow.AddDays(30),
            MessageJson = JsonConvert.SerializeObject(conversation),
            UserID = UserId,
        };

        var res = repository.Add(exportedConversation);
        return ResponseBase<ExportedConversation>.Success(res);
    }
}