namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class MentionedInArticleCommentEventHandler : INotificationHandler<MentionedInArticleCommentEvent>
{
    private readonly IArticleRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public MentionedInArticleCommentEventHandler(
        IArticleRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(MentionedInArticleCommentEvent notification, CancellationToken cancellationToken)
    {
        var articleCommentId = notification.ArticleComment.Id;
        var senderId = notification.SenderId;
        var recieverId = notification.RecieverId;


        var reciever = _repository
                        .Where<User>(a => a.Id == recieverId)
                        .FirstOrDefault();

        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();
        if (sender is null || reciever is null)
            return;

        var notif = new Notification()
        {
            NotificationType = NotificationType.MentionInArticleComment,
            IsReaded = false,
            Text = "you have been mentioned in article comment",
            ArticleCommentId = articleCommentId,
            SenderId = senderId,
            RecieverId = recieverId,
        };

        var notificationResult = _notificationService.Add(notif);
        if (notificationResult.Status != ResponseStatus.Success)
            return;
        if (!reciever.CommentNotification)
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