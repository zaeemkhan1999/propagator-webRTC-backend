namespace Apsy.App.Propagator.Application.Services;

public class SecretMessageService : ServiceBase<SecretMessage, SecretMessageInput>, ISecretMessageService
{
    public SecretMessageService(
        ISecretMessageRepository repository,
        IUserRepository userRepository,
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

    private readonly ISecretMessageRepository repository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITopicEventSender topicEventSender;
    private readonly IPublisher _publisher;

    //public  ListResponseBase<SecretMessage> Get(Expression<Func<SecretMessage, bool>> predicate = null, bool checkDeleted = false, User currentUser)
    //{

    //    if (currentUser == null)
    //        return ResponseStatus.UserNotFound;

    //    var chatMessages = repository.Where(c =>
    //                    (c.SenderId == currentUser.Id && !c.IsSenderDeleted)
    //                     ||
    //                    (c.ReceiverId == currentUser.Id && !c.IsReceiverDeleted));

    //    return new(chatMessages);
    //}

    //private void mapsterConfigForMessage()
    // {
    //    var currentUser = GetCurrentUser();

    //    _ = TypeAdapterConfig<SecretMessage, SecretMessageDto>
    //      .NewConfig()
    //      .Map(dest => dest.Post, src =>
    //                    (src.Post.Poster.Blocks.Any(x => x.BlockedId == currentUser.Id) ||
    //                    !src.Post.Poster.Followers.Any(x => x.FollowerId == currentUser.Id))
    //                    ? new Post { YourMind = "Not Allowed" } : src.Post)
    //    .Map(dest => dest.Article, src =>
    //                    (src.Article.User.Blocks.Any(x => x.BlockedId == currentUser.Id) ||
    //                    !src.Article.User.Followers.Any(x => x.FollowerId == currentUser.Id))
    //                    ? new Article { Title = "Not Allowed", SubTitle = "Not Allowed" } : src.Article)
    //    .Map(dest => dest.Story, src =>
    //                    (src.Story.User.Blocks.Any(x => x.BlockedId == currentUser.Id) ||
    //                    !src.Story.User.Followers.Any(x => x.FollowerId == currentUser.Id ||
    //                    !src.Story.User.HidedStory.Any(x => x.HidedId == currentUser.Id)
    //                    ))
    //                    ? new Story { Text = "Not Allowed" } : src.Story);
    //}


    public ResponseBase<SecretConversation> CreateConversation(
                                SecretConversationInput input)
    {

        SecretConversation secretConversation = input.Adapt<SecretConversation>();

        secretConversation.FirstUserId = input.SenderId;
        secretConversation.SecondUserId = input.ReceiverId;
        secretConversation.FirstUserPublicKey = input.PublicKey;
        secretConversation.IsBothUserJoinedToChat = false;
        secretConversation.LatestMessageDate = DateTime.UtcNow;

        return repository.Add(secretConversation);
    }


    public ResponseBase<SecretConversation> JoinToSecretChat(string publicKey, int secretConversationId, User currentUser)
    {

        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var secretConversation = repository.GetSecretConversation(secretConversationId).FirstOrDefault();

        if (secretConversation == null)
            return ResponseStatus.NotFound;

        //if (secretConversation.FirstUserPublicKey != publicKey && secretConversation.SecondUserPublicKey != publicKey)
        //    return ResponseStatus.NotAllowd;

        if (secretConversation.FirstUserId != currentUser.Id && secretConversation.SecondUserId != currentUser.Id)
            return ResponseStatus.NotAllowd;


        if (secretConversation.IsBothUserJoinedToChat)
            return secretConversation;


        if (secretConversation.FirstUserPublicKey == publicKey)
            return secretConversation;

        if (secretConversation.SecondUserPublicKey == publicKey)
            return secretConversation;

        secretConversation.IsBothUserJoinedToChat = true;
        secretConversation.SecondUserPublicKey = publicKey;
        return repository.Update(secretConversation);
    }



    public async Task<ResponseBase<SecretMessage>> CreateDirectMessage(
                                    SecretMessageInput messageInput,
                                    int senderId, int receiverId, User currentUser
                                   )
    {

        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var validation = messageInput.ValidateCreateMessage();
        if (validation.Status != ResponseStatus.Success)
            return validation;

        var secretConversation = repository.GetSecretConversationByFirstOrSecondPublickKey(messageInput.PublicKey);

        if (secretConversation == null)
        {
            return CustomResponseStatus.InvalidPublicKey;
        }
        if (secretConversation.FirstUserId != currentUser.Id && secretConversation.SecondUserId != currentUser.Id)
        {
            return ResponseStatus.NotAllowd;
        }
        if (!secretConversation.IsBothUserJoinedToChat)
        {
            return CustomResponseStatus.AnotherUserDontJoinedToSecretChat;
        }


        //SecretConversation secretConversation = (!(messageInput.SecretConversationId > 0)) ?
        //        repository
        //        .Where<SecretConversation>(x => (x.FirstUserId == (int?)senderId || x.FirstUserId == (int?)receiverId) && (x.SecondUserId == (int?)senderId || x.SecondUserId == (int?)receiverId))
        //            .FirstOrDefault()
        //        :
        //        repository
        //            .Where<SecretConversation>(a => a.Id == messageInput.SecretConversationId)
        //            .FirstOrDefault();



        //if (secretConversation == null)
        //{
        //    secretConversation = messageInput.Adapt<SecretConversation>();
        //    secretConversation.FirstUserId = senderId;
        //    secretConversation.SecondUserId = receiverId;
        //}

        if (receiverId == secretConversation.FirstUserId)
        {
            secretConversation.FirstUnreadCount++;
        }
        else
        {
            secretConversation.SecondUnreadCount++;
        }

        secretConversation.LatestMessageDate = DateTime.UtcNow;
        secretConversation = repository.Update(secretConversation);


        SecretMessage message = messageInput.Adapt<SecretMessage>();
        //if (secretConversation.Id > 0)
        //{
        message.SecretConversationId = secretConversation.Id;
        //}
        //else
        //{
        //    repository.Add(secretConversation);
        //    message.SecretConversation = secretConversation;
        //}

        message.SenderId = senderId;
        int? reciverId = (secretConversation.FirstUserId == message.SenderId) ? secretConversation.SecondUserId : secretConversation.FirstUserId;
        var receiver = _userRepository.GetUser(reciverId ?? 0).FirstOrDefault();// repository.Find<User>((int)reciverId);
        if (receiver == null)
            return ResponseStatus.UserNotFound;

        SecretMessage result = repository.Add(message);
        //if (message.MessageType == MessageType.Post)
        //{
        //    var post = repository.Find<Post>((int)messageInput.PostId);
        //    if (post != null)
        //    {
        //        post.ShareCount++;
        //        repository.Update(post);
        //    }
        //}

        //if (message.MessageType == MessageType.Article)
        //{
        //    var article = repository.Find<Article>((int)messageInput.ArticleId);
        //    if (article != null)
        //    {
        //        article.ShareCount++;
        //        repository.Update(article);
        //    }
        //}

        if (receiver.DirectNotification)
            await SendDirectMessageSubscriptionAsync(result, senderId, (int)reciverId);
        return ResponseBase<SecretMessage>.Success(result);
    }


    public ResponseStatus Remove(int entityId, int userId)
    {
        SingleResponseBase<SecretMessage> singleResponseBase = Get(entityId, (SecretMessage a) => a.SecretConversation);
        SecretConversation byId = repository.GetSecretConversation(singleResponseBase.Result.SecretConversationId).FirstOrDefault();//repository.GetById<SecretConversation>(new object[1] { singleResponseBase.Result.SecretConversationId });
        if (singleResponseBase.Status != ResponseStatus.Success)
        {
            return singleResponseBase.Status;
        }

        if (singleResponseBase.Result.SecretConversation.FirstUserId != userId && singleResponseBase.Result.SecretConversation.SecondUserId != userId)
        {
            return ResponseStatus.NotAllowd;
        }

        return SoftDelete(entityId);
    }

    public async Task<ResponseBase<bool>> SeenMessages(List<int> messagesIds)
    {
        //var messages = await repository.Where<SecretMessage>(c => messagesIds.Contains(c.Id)).ToListAsync();
        var messages = await repository.SeenMessages(messagesIds);
        foreach (var item in messages)
        {
            item.IsSeen = true;
            item.SeenDate = DateTime.UtcNow;
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



    public ResponseStatus RemoveConversation(int conversationId, int userId, string publicKey, User currentUser)
    {
        SecretConversation conversation = repository.GetSecretConversationByIdAndPublicKey(conversationId, publicKey);

        if (conversation == null)
        {
            return ResponseStatus.NotFound;
        }


        if (conversation.FirstUserId != userId && conversation.SecondUserId != userId)
        {
            return ResponseStatus.NotAllowd;
        }
        if (conversation.FirstUserPublicKey != publicKey && conversation.SecondUserPublicKey != publicKey)
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

            //var messages = repository.Where<SecretMessage>(c => c.SecretConversationId == conversationId
            //                            &&
            //                            (c.SenderId == currentUser.Id || c.ReceiverId == currentUser.Id));
            var messages = repository.GetSecretMessage(conversationId, currentUser.Id);

            var chatMessages = new List<SecretMessage>();
            foreach (var message in messages)
            {

                if (message.SenderId == currentUser.Id)
                    message.IsSenderDeleted = true;
                else if (message.ReceiverId == currentUser.Id)
                    message.IsReceiverDeleted = true;

                chatMessages.Add(message);
            }

            if (conversation.FirstUserId == currentUser.Id && conversation.FirstUserPublicKey == publicKey)
            {
                conversation.IsFirstUserDeleted = true;
                conversation.FirstUserDeletedDate = DateTime.UtcNow;
            }
            if (conversation.SecondUserId == currentUser.Id && conversation.FirstUserPublicKey == publicKey)
            {
                conversation.SecondUserDeletedDate = DateTime.UtcNow;
                conversation.IsSecondUserDeleted = true;
            }
            repository.UpdateRange(chatMessages);
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

        //SecretMessage message = repository.GetById(messageId, checkDeleted: true);
        SecretMessage message = repository.GetSecretMessageById(messageId, true);
        if (message == null)
        {
            return ResponseStatus.NotFound;
        }

        SecretConversation conversation = repository.GetSecretConversation(message.SecretConversationId).FirstOrDefault(); //repository.GetById<SecretConversation>(new object[1] { message.SecretConversationId });

        if (message.IsDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }


        if (message.SenderId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        SecretMessage secretMessage = repository.Remove(message);
        if (secretMessage.IsDeleted)
        {
            return ResponseStatus.Success;
        }

        return ResponseStatus.Failed;
    }
    private async ValueTask SendDirectMessageSubscriptionAsync(SecretMessage message, int senderId, int reciverId)
    {
        try
        {
            await _publisher.Publish(new SendSecretDirectMessageEvent(message, senderId, reciverId));
        }
        catch
        {
        }
        // await topicEventSender.SendAsync($"{receiverId}_DirectMessageAdded", message);
    }

}
