using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class NotificationQueries
{
    [GraphQLName("notification_getNotification")]
    public SingleResponseBase<Notification> Get(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] INotificationService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("notification_getNotifications")]
    public ListResponseBase<Notification> GetItems(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service(ServiceKind.Default)] INotificationService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }

    [GraphQLName("notification_getMyNotifications")]
    public ListResponseBase<NotificationDto> GetMyNotifications(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service(ServiceKind.Default)] INotificationReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetNotifs(authentication.CurrentUser);
    }
}