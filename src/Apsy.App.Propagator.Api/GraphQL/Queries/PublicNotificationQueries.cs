using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PublicNotificationQueries
{
    [GraphQLName("publicNotification_getPublicNotification")]
    public SingleResponseBase<PublicNotification> GetPublicNotification(
                        [Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IPublicNotificationReadService service,
                        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        return service.GetPublicNotification(entityId);
    }

    [GraphQLName("publicNotification_getPublicNotificationes")]
    public ListResponseBase<PublicNotification> GetPublicNotificationes(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPublicNotificationReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        return service.GetPublicNotifications();
    }



}