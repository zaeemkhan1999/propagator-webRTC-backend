namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class PublicNotificationMutations
{
    [GraphQLName("publicNotification_createPublicNotification")]
    public async Task<ResponseBase<PublicNotification>> Create(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPublicNotificationService service,
        PublicNotificationInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.AddPublicNotification(input);
    }


    [GraphQLName("publicNotification_removePublicNotification")]
    public async Task<ResponseStatus> Remove(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] IPublicNotificationService service,
                    int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;


        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.DeletePublicNotification(entityId);
    }

    [GraphQLName("publicNotification_updatePublicNotification")]
    public async Task<ResponseBase<PublicNotification>> Update(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPublicNotificationService service,
        PublicNotificationInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.UpdatePublicNotification(input);
    }

    [GraphQLName("publicNotification_sendPublicNotification")]
    public async Task<ResponseBase> Send(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IPublicNotificationService service,
        int notificationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.Send(notificationId);
    }

}