namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class NotificationMutations
{
    [GraphQLName("notification_sendNotificationToAllUsers")]
    public async Task<ListResponseBase<Notification>> SendNotificationToAllUsers(
            int notificationId,
            [Authentication] RequestInterception.Authentication authentication,
            [Service] INotificationService notificationService,
            NotificationInput input)
    {
        //if (authentication.Status != ResponseStatus.Success)
        //{
        //    return authentication.Status;
        //}

        //User currentUser = authentication.CurrentUser;
        //if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await notificationService.SendNotificationToAllUsers(input,authentication.CurrentUser);
    }

    [GraphQLName("notification_deleteNotification")]
    public ResponseBase<Notification> DeleteNotification(
            int notificationId,
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] INotificationService notificationService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return notificationService.SoftDelete(notificationId);
    }

    [GraphQLName("notification_readNotification")]
    public ResponseBase<Notification> ReadNotification(
               int notificationId,
               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
               [Service] INotificationService notificationService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return notificationService.ReadNotification(notificationId, currentUser);
    }

    [GraphQLName("notification_readNotifications")]
    public ResponseBase<bool> ReadNotifications(
               int[] notificationIds,
               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
               [Service] INotificationService notificationService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return notificationService.ReadNotifications(notificationIds, currentUser);
    }

    [GraphQLName("notification_readNotificationCurrentUser")]
    public async Task<ResponseBase<bool>> ReadNotificationCurrent(
               int[] notificationIds,
               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
               [Service] INotificationService notificationService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return await notificationService.ReadNotificationCurrent(currentUser);
    }
}