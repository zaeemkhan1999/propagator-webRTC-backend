using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common;
using Apsy.App.Propagator.Infrastructure.Redis;
using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class MessageReadService : ServiceBase<Message, MessageInput>, IMessageReadService
    {
        private readonly IRedisCacheService _redisCache;
        private readonly IBlockUserReadRepository _blockUserRepository;
        private readonly IMessageReadRepository repository;
        private readonly IHideStoryReadRepository _hideStoryRepository;
        private readonly IArticleReadRepository _articleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITopicEventSender topicEventSender;
        private readonly IPublisher _publisher;
        private readonly IInterestedUserService interestedUserService;
        private readonly IGroupTopicReadRepository _groupTopicRepository;
        private readonly IExportConversationReadRepository _exportConversation;
        public MessageReadService(
        IRedisCacheService redisCache,
        IMessageReadRepository repository,
        IHttpContextAccessor httpContextAccessor,
        ITopicEventSender topicEventSender,
        IPublisher publisher,
        IExportConversationReadRepository conversationRepository,
        IArticleReadRepository articleRepository,
        IHideStoryReadRepository hideStoryRepository,
        IBlockUserReadRepository blockUserRepository,
        IInterestedUserService interestedUserService,
        IGroupTopicReadRepository groupTopicRepository) : base(repository)
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
        public override ListResponseBase<Message> Get(Expression<Func<Message, bool>> predicate = null, bool checkDeleted = false)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.UserNotFound;
            var chatMessages = repository.GetMessages().Where(c => c.Conversation.IsGroup || c.SenderId == currentUser.Id || c.ReceiverId == currentUser.Id);
            return new(chatMessages);
        }
        public ListResponseBase<MessageDto> GetDirectMessages(int? userId, int conversationid, User currentUser)
        {
            if (userId.HasValue)
                currentUser.Id = userId.GetValueOrDefault();
            
            mapsterConfigForMessage(currentUser);
            
            var chatMessages = repository.GetMessages(conversationid,currentUser);
            var list = chatMessages.ToList();

            foreach (var item in list)
            {
                if ((item.CreatedDate.AddDays(30) - DateTime.Now).Days < 1)
                {

                    item.DaysRemainings = (item.CreatedDate.AddDays(30) - DateTime.Now).Hours.ToString();
                }
                else
                {
                    item.DaysRemainings = (item.CreatedDate.AddDays(30) - DateTime.Now).Days.ToString();
                }
                                    
            };

            return new(list.AsQueryable());
        }
        public ListResponseBase<MessageDto> GetGroupMessages(User user)
        {
            mapsterConfigForMessage(user);
            var chatMessages = repository.GetMessages().Where(c => c.Conversation.IsGroup).ProjectToType<MessageDto>();
            return new(chatMessages);
        }
        private void mapsterConfigForMessage(User currentUser)
        {

            _ = TypeAdapterConfig<Message, MessageDto>
              .NewConfig()
              .Map(dest => dest.Post, src =>
                            src.Post.Poster.Id != currentUser.Id &&
                            (src.Post.Poster.Blocks.Any(x => x.BlockedId == currentUser.Id) ||
                             src.Post.Poster.PrivateAccount && !src.Post.Poster.Followers.Any(x => x.FolloweAcceptStatus == FolloweAcceptStatus.Accepted && x.FollowerId == currentUser.Id))
                            ? new Post { YourMind = "Not Allowed", Poster = new User { Username = src.Post.Poster.Username } } : src.Post)
            .Map(dest => dest.Article, src =>
                            src.Article.User.Id != currentUser.Id &&
                            (src.Article.User.Blocks.Any(x => x.BlockedId == currentUser.Id) ||
                             src.Article.User.PrivateAccount && !src.Article.User.Followers.Any(x => x.FolloweAcceptStatus == FolloweAcceptStatus.Accepted && x.FollowerId == currentUser.Id))
                            ? new Article { Title = "Not Allowed", SubTitle = "Not Allowed", User = new User { Username = src.Article.User.Username } } : src.Article)
            .Map(dest => dest.Story, src =>
                            src.Story.User.Id != currentUser.Id &&
                            (src.Story.User.Blocks.Any(x => x.BlockedId == currentUser.Id) ||
                            src.Story.User.PrivateAccount && !src.Story.User.Followers.Any(x => x.FolloweAcceptStatus == FolloweAcceptStatus.Accepted && x.FollowerId == currentUser.Id) ||
                            src.Story.User.HidedStory.Any(x => x.HidedId == currentUser.Id)
                            )
                            ? new Story { Text = "Not Allowed", User = new User { Username = src.Story.User.Username } } : src.Story);
        }
        public SingleResponseBase<ConversationDto> GetGroup(int conversationId, User currentUser)
        {
            IQueryable<ConversationDto> queryable = repository.GroupList(conversationId, currentUser);
            if (queryable.Any())
            {
                return new SingleResponseBase<ConversationDto>(queryable);
            }
            return ResponseStatus.NotFound;
        }
        public ListResponseBase<ConversationDto> GetGroups(int? userId, User currentUser)
        {
            if (userId.HasValue)
                currentUser.Id = userId.GetValueOrDefault();
            var conversations = repository.GetConversationGroups(currentUser.Id);
            return new(conversations);
        }
        public SingleResponseBase<Conversation> GetConversation(int conversationId, int userId)
        {
            if (conversationId <= 0)
            {
                return SingleResponseBase<Conversation>.Failure(ResponseStatus.NotEnoghData);
            }

            Conversation val = repository.GetConversation(conversationId);
            if (val == null)
            {
                return SingleResponseBase<Conversation>.Failure(ResponseStatus.NotEnoghData);
            }

            if (val.FirstUserId == userId)
            {
                val.FirstUnreadCount = 0;
            }
            else if (val.SecondUserId == userId)
            {
                val.SecondUnreadCount = 0;
            }
            else
            {
                if (!val.UserGroups.Any((x) => x.UserId == userId))
                {
                    return SingleResponseBase<Conversation>.Failure(ResponseStatus.NotAllowd);
                }

                IEnumerable<UserMessageGroup> enumerable = val.UserGroups.Where((x) => x.UserId == userId);
                UserMessageGroup val2 = val.UserGroups.FirstOrDefault((x) => x.UserId == userId);
                val2.UnreadCount = 0;
            }

            repository.Update(val);
            IQueryable<Conversation> result = (IQueryable<Conversation>)repository.GetConversation(conversationId);
            return SingleResponseBase<Conversation>.Success(result);
        }

        public ListResponseBase<ConversationDto> GetUserMessages(int userId)
        {
            var conversations = repository.GetConversations(userId);
            return ListResponseBase<ConversationDto>.Success(conversations);
        }
        public async Task<ListResponseBase<UserMessageGroup>> GetGroupMembers(int conversationId, User user)
        {
            var conversation = await repository.GetConversationById(conversationId);
            if (conversation is null)
                return ResponseStatus.NotFound;

            var isAmemberOfGroup = repository.GetMessageGroups().Any(c => c.UserId == user.Id && c.ConversationId == conversationId);
            if (!isAmemberOfGroup)
                return CustomMessagingResponseStatus.YouAreNotAMemberOfTheGroup;

            var groupMember = repository.GetUserMessageGroups(conversationId);
            return new(groupMember);
        }

        public ListResponseBase<FollowUserForGroupDto> GetMyFollowerNotInvitedToGroup(int? conversationId, User currentUser)
        {
            return new(repository.GetMyFollowerNotInvitedToGroup(conversationId, currentUser));
        }

        public ListResponseBase<FollowUserForGroupDto> GetUsersNotInvitedToGroup(int? conversationId, User currentUser)
        {
            var query = repository.GetUsersNotInvitedToGroup(conversationId, currentUser);
            return new ListResponseBase<FollowUserForGroupDto>(query);
        }

        public SingleResponseBase<Conversation> GetConversationWithOtherUser(int otherUserId, User currentUser)
        {
            var conversations = repository.GetConversation(otherUserId, currentUser);
            return SingleResponseBase<Conversation>.Success(conversations);
        }

        public ListResponseBase<DiscussionsDto> GetDiscussions(User currentUser)
        {
            var result = repository.GetDiscussions(currentUser);
            return new ListResponseBase<DiscussionsDto>(result);

        }
        private User GetCurrentUser()
        {
            var User = _httpContextAccessor.HttpContext.User;
            if (!User.Identity.IsAuthenticated)
                return null;

            var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
            var user = JsonConvert.DeserializeObject<User>(userString);
            return user;
        }
        public ResponseBase<Conversation> GetConversationBySubscriptionPlanId(int subscriptionPlanId)
        {
            var conversation = repository.GetConversationBySubscriptionPlanId(subscriptionPlanId);
            return conversation;
        }
        public async Task<ResponseBase<ExportedConversation>> GetExportedChat(int Id)
        {
            var res = await _exportConversation.GetAsync(Id);
            return ResponseBase<ExportedConversation>.Success(res);
        }
        public async ValueTask<ListResponseBase<GroupTopic>> GetGroupTopics(int conversationId, User currentUser)
        {
            var conversation = repository.GetConversation(conversationId);

            if (conversation == null)
            {
                return CustomMessagingResponseStatus.ConversationNotExist;
            }
            // If the current user is the owner or admin then there is no need to check the database to find out if it is the admin of the group or not
            // if (currentUser.UserTypes == UserTypes.User)
            // {
            //     var isCurrentUserAdmin = repository.GetUserMessageGroups(conversationId, currentUser.Id).Any();

            //     if (!isCurrentUserAdmin)
            //     {
            //         return CustomMessagingResponseStatus.UserNotBelongToGroup;
            //     }
            // }
            var result = await Task.Run(() => _groupTopicRepository.GetGroupTopicByConversationId(conversationId));
            return ListResponseBase<GroupTopic>.Success(result);
        }
        public ResponseBase<bool> IsGroupLinkExist(string groupLink) =>
                                       repository.ConversationGetAll().Any(x => x.GroupLink != null && x.GroupLink == groupLink);

        public ResponseBase<string> GenerateUniqueKeyForGroup()
        {
            var uniqueKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            var linkExist = repository.ConversationGetAll().Any(x => x.GroupLink == uniqueKey);
            if (linkExist)
                return GenerateUniqueKeyForGroup();

            return uniqueKey;
        }
    }
}
