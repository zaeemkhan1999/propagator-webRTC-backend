namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class SendDirectMessageEventHandler : INotificationHandler<SendDirectMessageEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public SendDirectMessageEventHandler(
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(SendDirectMessageEvent notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var receiverId = notification.RecieverId;
        var senderId = notification.SenderId;
        if (message is null)
            return;

        var notif = new Notification()
        {
            NotificationType = NotificationType.Message,
            IsReaded = false,
            Text = $"{message.Text}",
            MessageId = message.Id,
            SenderId = senderId,
            RecieverId = receiverId,
        };

        var msg = new Message
        {
            Id = message.Id,
            MessageType = MessageType.Text,
            Text = $"{message.Text}",
            SenderId = senderId.GetValueOrDefault(),
            ReceiverId = receiverId
        };

        try
        {
            await _sender.SendAsync($"{receiverId}_Subcription", new SubscriptionDto(msg));
            await _notificationService.SendFirebaseCloudMessage(notif);
        }
        catch (Exception)
        {
        }
    }
}