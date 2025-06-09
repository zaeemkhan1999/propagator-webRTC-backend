using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class ArticleCommentQueries
{
    [GraphQLName("articleComment_getArticleComment")]
    public SingleResponseBase<ArticleCommentDto> Get(
                                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                        [Service(ServiceKind.Default)] IArticleCommentReadService service,
                                        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetArticleComment(entityId,authentication.CurrentUser);
    }

    [GraphQLName("articleComment_getArticleComments")]
    public ListResponseBase<ArticleCommentDto> GetItems(
                                       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                       [Service(ServiceKind.Default)] IArticleCommentReadService service,
                                      bool loadDeleted)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.GetArticleComments(loadDeleted, authentication.CurrentUser);
    }
}
