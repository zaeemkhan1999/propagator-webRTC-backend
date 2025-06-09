namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class SecretMessageMutations
{
    [GraphQLName("message_createSecretDirectMessage")]
    public async Task<ResponseBase<SecretMessage>> CreateDirectMessage(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] ISecretMessageService service,
                    SecretMessageInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.SenderId = currentUser.Id;
        return await service.CreateDirectMessage(input, currentUser.Id, (int)input.ReceiverId, authentication.CurrentUser);
    }

    [GraphQLName("message_createSecretConversation")]
    public ResponseBase<SecretConversation> CreateSecretConversation(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISecretMessageService service,
        SecretConversationInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.CreateConversation(input);
    }


    [GraphQLName("message_joinToSecretChat")]
    public ResponseBase<SecretConversation> JoinToSecretChat(
                              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                              [Service(ServiceKind.Default)] ISecretMessageService service,
                              string publicKey,
                              int secretConversationId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.JoinToSecretChat(publicKey, secretConversationId,authentication.CurrentUser);
    }

    [GraphQLName("message_removeSecretConversation")]
    public virtual ResponseStatus RemoveSecretConversation(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISecretMessageService service,
        int conversationId, string publicKey)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.RemoveConversation(conversationId, currentUser.Id, publicKey, authentication.CurrentUser);
    }

    [GraphQLName("message_removeSecretMessage")]
    public virtual ResponseStatus RemoveSecretMessage(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] ISecretMessageService service,
        int messageId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.RemoveMessage(messageId, authentication.CurrentUser);
    }

    //[GraphQLName("message_updateSecretMessage")]
    //public virtual ResponseBase<SecretMessage> UpdateSecretMessage(
    //    [Authentication] RequestInterception.Authentication authentication,
    //    [Service(ServiceKind.Default)] ISecretMessageService service,
    //    string text,
    //    int messageId)
    //{
    //    if (authentication.Status != ResponseStatus.Success)
    //    {
    //        return authentication.Status;
    //    }

    //    User currentUser = authentication.CurrentUser;
    //    return service.UpdateMessage(text, messageId, currentUser.Id);
    //}

    [GraphQLName("message_seenSecretMessages")]
    public async Task<ResponseBase<bool>> SeenSecretMessages(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] ISecretMessageService service,
                    List<int> messagesIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.SeenMessages(messagesIds);
    }

}