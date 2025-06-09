namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class SendSecretDirectMessageEventHandler : INotificationHandler<SendSecretDirectMessageEvent>
{

    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public SendSecretDirectMessageEventHandler(
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(SendSecretDirectMessageEvent notification, CancellationToken cancellationToken)
    {
        var message = notification.SecretMessage;
        var receiverId = notification.RecieverId;
        var senderId = notification.SenderId;
        if (message is null)
            return;

        var notif = new Notification()
        {
            NotificationType = NotificationType.SecretMessage,
            IsReaded = false,
            Text = $"new secret message",
            MessageId = message.Id,
            SenderId = senderId,
            RecieverId = receiverId,
        };

        //var notificationResult = _notificationService.Add(notif);
        //if (notificationResult.Status != ResponseStatus.Success)
        //    return;

        try
        {
            await _sender.SendAsync($"{receiverId}_Subcription", new SubscriptionDto(notif));
            await _notificationService.SendFirebaseCloudMessage(notif);
        }
        catch
        {
        }
    }
}