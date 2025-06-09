namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class AddArticleCommentEventHandler : INotificationHandler<AddArticleCommentEvent>
{
    private readonly IArticleRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public AddArticleCommentEventHandler(
        IArticleRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(AddArticleCommentEvent notification, CancellationToken cancellationToken)
    {
        var articleCommentId = notification.ArticleCommentId;
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
            NotificationType = NotificationType.ArticleComment,
            IsReaded = false,
            Text = $"{senderIdentifier} left a comment to your article:",
            ArticleCommentId = articleCommentId,
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