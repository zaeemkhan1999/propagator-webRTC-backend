using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common.Dtos;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]

    public class ExportConversationQueries
    {


    [GraphQLName("conversation_Getexport")]
    public async Task<ExportedConversationDto> conversation_Getexport(
                        [Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IExportConversationReadService service,
                        int entityId)
    {
        return await service.GetExportedChat(entityId);
    }
}

