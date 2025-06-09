namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class AddUserToGroupEventHandler : INotificationHandler<AddUserToGroupEvent>
{
    private readonly IMessageRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public AddUserToGroupEventHandler(
        IMessageRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(AddUserToGroupEvent notification, CancellationToken cancellationToken)
    {
        //var userId = notification.userId;
        var conversationId = notification.conversationId;
        var senderId = notification.SenderId;
        var recieverId = notification.RecieverId;
        var isForAdmin = notification.isForAdmin;

        //senderId is userId, when notification send for admin
        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();
        if (sender is null)
            return;

        var conversation = _repository
                       .Where<Conversation>(a => a.Id == conversationId)
                       .FirstOrDefault();
        if (conversation is null)
            return;

        var senderIdentifier = string.IsNullOrEmpty(sender.Username) ? sender.Email : sender.Username;
        var groupName = string.IsNullOrEmpty(conversation.GroupName) ? "" : conversation.GroupName;
        var notif = new Notification();
        if (isForAdmin)
        {
            //NOTIFICTION FOR ADMIN
            notif.NotificationType = NotificationType.NewUserAddedToPrivateGroup;
            notif.IsReaded = false;
            notif.Text = $"User {senderIdentifier} joined the group {groupName}";
            notif.ConversationId = conversationId;
            notif.SenderId = senderId;
            notif.RecieverId = recieverId;
        }
        else
        {
            //Notification for user
            notif.NotificationType = NotificationType.NewUserAddedToPrivateGroup;
            notif.IsReaded = false;
            notif.Text = $"You have been successfully added to the group {groupName}";
            notif.ConversationId = conversationId;
            notif.SenderId = senderId;
            notif.RecieverId = recieverId;
        }

        var notificationResult = _notificationService.Add(notif);
        if (notificationResult.Status != ResponseStatus.Success)
            return;
        try
        {
            await _sender.SendAsync($"{notif.RecieverId}_Subcription", new SubscriptionDto(notificationResult.Result));
            await _notificationService.SendFirebaseCloudMessage(notif);
        }
        catch
        {
        }
    }
}