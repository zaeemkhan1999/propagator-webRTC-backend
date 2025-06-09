
using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class CommentQueries
{
    [GraphQLName("comment_getComment")]
    public SingleResponseBase<CommentDto> GetComment(
                                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                    [Service] ICommentReadService service,
                                    int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetComment(entityId, authentication.CurrentUser);
    }

    [GraphQLName("comment_getComments")]
    public ListResponseBase<CommentDto> GetComments(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service(ServiceKind.Default)] ICommentReadService service,
                bool loadDeleted)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.GetComments(loadDeleted, authentication.CurrentUser);
    }
}

