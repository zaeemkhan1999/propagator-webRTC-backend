namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class StrikeEventHandler : INotificationHandler<StrikeEvent>
{
    private readonly IArticleRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public StrikeEventHandler(
        IArticleRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(StrikeEvent notification, CancellationToken cancellationToken)
    {
        var postId = notification.PostId;
        var articleId = notification.ArticleId;
        var senderId = notification.SenderId;
        var recieverId = notification.RecieverId;

        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();
        if (sender is null)
            return;

        var senderIdentifier = string.IsNullOrEmpty(sender.Username) ? sender.Email : sender.Username;
        var message = $"Your post received a strike from our admin. Three strikes will result in a five day ban.";
        if (articleId is not null)
            message = $"Your article received a strike from our admin. Three strikes will result in a five day ban.";

        var notif = new Notification()
        {
            NotificationType = postId is not null ? NotificationType.StrikePost : NotificationType.StrikeArticle,
            IsReaded = false,
            Text = message,
            PostId = postId,
            Articled = articleId,
            SenderId = senderId,
            RecieverId = recieverId,
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