namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class AdminSentEventHandler : INotificationHandler<AdminSentEvent>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public AdminSentEventHandler(
        INotificationRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(AdminSentEvent notification, CancellationToken cancellationToken)
    {
        var senderId = notification.SenderId;
        var recieverId = notification.RecieverId;

        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();

        if (sender is null)
            return;

        try
        {
            await _sender.SendAsync($"{notification.Notification.RecieverId}_Subcription", new SubscriptionDto(notification.Notification));
            await _notificationService.SendFirebaseCloudMessage(notification.Notification);
        }
        catch
        {
        }
    }


}