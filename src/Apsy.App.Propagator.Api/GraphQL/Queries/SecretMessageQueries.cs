using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SecretMessageQueries
{
    [GraphQLName("message_getSecretMessage")]
    public SingleResponseBase<SecretMessage> GetSecretMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISecretMessageReadService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("message_getDirectSecretMessage")]
    public ListResponseBase<SecretMessageDto> GetDirectSecretMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISecretMessageReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetDirectMessages(authentication.CurrentUser);
    }

    [GraphQLName("message_getSecretConversation")]
    public SingleResponseBase<SecretConversation> GetSecretConversation(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] ISecretMessageReadService messageService,
                            int conversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return messageService.GetConversation(conversationId, currentUser.Id);
    }

    [GraphQLName("message_getSecretUserMessages")]
    public ListResponseBase<SecretConversationDto> GetSecretUserMessages(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISecretMessageReadService messageService,
        string publicKey)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return messageService.GetUserMessages(currentUser.Id, publicKey, authentication.CurrentUser);
    }

    [GraphQLName("message_getSecretConversationWithOtherUser")]
    public SingleResponseBase<SecretConversation> GetSecretConversationWithOtherUser(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service] ISecretMessageReadService service,
            int otherUserId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.GetConversationWithOtherUser(otherUserId, currentUser);
    }
}