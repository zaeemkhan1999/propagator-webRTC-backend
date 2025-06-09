namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IPublicNotificationService : IServiceBase<PublicNotification, PublicNotificationInput>
{
    Task<ResponseBase<PublicNotification>> AddPublicNotification(PublicNotificationInput input);
    Task<ResponseBase> Send(int notificationId);
    Task<ResponseBase<PublicNotification>> UpdatePublicNotification(PublicNotificationInput input);
    Task<ResponseBase> DeletePublicNotification(int id);
}