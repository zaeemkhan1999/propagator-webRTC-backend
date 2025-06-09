namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class LikePostEventHandler : INotificationHandler<LikePostEvent>
{
    private readonly IArticleRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public LikePostEventHandler(
        IArticleRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(LikePostEvent notification, CancellationToken cancellationToken)
    {
        var postLikeId = notification.PostLikeId;
        var senderId = notification.SenderId;
        var recieverId = notification.RecieverId;
        var postId = notification.PostId;
       

        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();
        if (sender is null)
            return;

        var senderIdentifier = string.IsNullOrEmpty(sender.Username) ? sender.Email : sender.Username;
        var notif = new Notification()
        {
            NotificationType = NotificationType.PostLike,
            IsReaded = false,
            Text = $"{senderIdentifier} liked your post:",
            PostLikeId = postLikeId,
            SenderId = senderId,
            RecieverId = recieverId,
            PostId = postId,
        };

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