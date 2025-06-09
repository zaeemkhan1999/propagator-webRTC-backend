namespace Apsy.App.Propagator.Application.Primitive.DomainEvent;

public class RejectAdsEventEventHandler : INotificationHandler<RejectAdsEvent>
{
    private readonly IAdReadRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ITopicEventSender _sender;

    public RejectAdsEventEventHandler(
        IAdReadRepository repository,
        INotificationService notificationService,
        ITopicEventSender sender)
    {
        _repository = repository;
        _notificationService = notificationService;
        _sender = sender;
    }

    public async Task Handle(RejectAdsEvent rejectAdsEvent, CancellationToken cancellationToken)
    {
        var adsId = rejectAdsEvent.AdsId;
        var senderId = rejectAdsEvent.SenderId;
        var recieverId = rejectAdsEvent.RecieverId;

        var sender = _repository
                        .Where<User>(a => a.Id == senderId)
                        .FirstOrDefault();
        if (sender is null)
            return;

        var ads = _repository.Where(d => d.Id == adsId).FirstOrDefault();
        var senderIdentifier = string.IsNullOrEmpty(sender.Username) ? sender.Email : sender.Username;
        var notif = new Notification()
        {
            NotificationType = NotificationType.RejectAds,
            IsReaded = false,
            Text = $"You ad (ticket number:{ads.TicketNumber}) has been rejected.",
            AdsId = adsId,
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