namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class UnFollowRequestEventHandler : INotificationHandler<UnFollowRequestEvent>
{
    private readonly IArticleRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public UnFollowRequestEventHandler(
        IArticleRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(UnFollowRequestEvent notification, CancellationToken cancellationToken)
    {
        var userFollowerId = notification.UserFollowerId;
        var senderId = notification.SenderId;
        var recieverId = notification.RecieverId;

        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();
        if (sender is null)
            return;

        var senderIdentifier = string.IsNullOrEmpty(sender.Username) ? sender.Email : sender.Username;
        var notif = new Notification()
        {
            NotificationType = NotificationType.UnFollow,
            IsReaded = false,
            Text = $"{senderIdentifier} unfollowed you:",
            UserFollowerId = userFollowerId,
            SenderId = senderId,
            RecieverId = recieverId,
        };

        var notificationResult = _notificationService.Add(notif);
        if (notificationResult.Status != ResponseStatus.Success)
            return;

        try
        {
            await _sender.SendAsync($"{notif.RecieverId}_Subcription", new SubscriptionDto(notif));
            await _notificationService.SendFirebaseCloudMessage(notif);
        }
        catch
        {
        }
    }
}