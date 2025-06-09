using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class SecretMessageReadService: ServiceBase<SecretMessage, SecretMessageInput>, ISecretMessageReadService
    {
        private readonly ISecretMessageReadRepository repository;
        private readonly IUserReadRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITopicEventSender topicEventSender;
        private readonly IPublisher _publisher;
        public SecretMessageReadService(
        ISecretMessageReadRepository repository,
        IUserReadRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        ITopicEventSender topicEventSender,
        IPublisher publisher) : base(repository)
        {
            this.repository = repository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            this.topicEventSender = topicEventSender;
            _publisher = publisher;
        }
        public ListResponseBase<SecretMessageDto> GetDirectMessages(User currentUser)
        {
            if (currentUser == null)
                return ResponseStatus.UserNotFound;

            //mapsterConfigForMessage();
            //var chatMessages = repository.Where(c =>
            //              (c.SenderId == currentUser.Id && !c.IsSenderDeleted) ||
            //              (c.ReceiverId == currentUser.Id && !c.IsReceiverDeleted))
            //        .ProjectToType<SecretMessageDto>();
            var chatMessages = repository.GetDirectMessages(currentUser.Id);
            return new(chatMessages);
        }
        public SingleResponseBase<SecretConversation> GetConversation(int conversationId, int userId)
        {
            if (conversationId <= 0)
            {
                return ResponseStatus.NotEnoghData;
            }

            SecretConversation secretConversation = repository.GetSecretConversation(conversationId).FirstOrDefault(); //repository.Where((SecretConversation a) => a.Id == conversationId).FirstOrDefault();
            if (secretConversation == null)
            {
                return ResponseStatus.NotEnoghData;
            }

            if (secretConversation.FirstUserId == userId)
            {
                secretConversation.FirstUnreadCount = 0;
            }
            else if (secretConversation.SecondUserId == userId)
            {
                secretConversation.SecondUnreadCount = 0;
            }
            //else
            // {
            //    if (!val.UserGroups.Any((UserMessageGroup x) => x.UserId == userId))
            //    {
            //        return ResponseStatus.NotAllowd;
            //    }

            //    IEnumerable<UserMessageGroup> enumerable = val.UserGroups.Where((UserMessageGroup x) => x.UserId == userId);
            //    UserMessageGroup val2 = val.UserGroups.FirstOrDefault((UserMessageGroup x) => x.UserId == userId);
            //    val2.UnreadCount = 0;
            //}

            repository.Update(secretConversation);
            IQueryable<SecretConversation> result = repository.GetSecretConversation(conversationId); //repository.Where((SecretConversation a) => a.Id == conversationId);
            return SingleResponseBase<SecretConversation>.Success(result);
        }
        public ListResponseBase<SecretConversationDto> GetUserMessages(int userId, string publicKey, User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.UserNotFound;

            var user = _userRepository.GetUser(userId); //repository.Where<User>(u => u.Id == userId).AsNoTracking().FirstOrDefault();
            var conversations = repository.GetUserMessages(userId, currentUser.Id, publicKey);

            return ListResponseBase<SecretConversationDto>.Success(conversations);
        }

        public SingleResponseBase<SecretConversation> GetConversationWithOtherUser(int otherUserId, User currentUser)
        {
            var conversations = repository.GetSecretConversationWithOtherUser(currentUser.Id, otherUserId);

            return SingleResponseBase<SecretConversation>.Success(conversations);
        }
    }
}
