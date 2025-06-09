namespace Apsy.App.Propagator.Api.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class ExportConversationMutation
    {
        [GraphQLName("conversation_export")]
        public async Task<ResponseBase<ExportedConversationDto>> ExportChat(
                       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                       [Service(ServiceKind.Default)] IExportConversationService service,
                       int ConversationId)
        {
            if (authentication.Status != ResponseStatus.Success)
            {
                return authentication.Status;
            }

            return await service.ExportChat(ConversationId, authentication.CurrentUser.Id);
        }

    }
}
