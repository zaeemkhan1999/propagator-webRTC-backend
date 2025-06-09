namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class LikeArticleCommentQueries
{
    [GraphQLName("likeArticleComment_getLikedArticleComment")]
    public SingleResponseBase<LikeArticleComment> Get(
                                              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                         [Service(ServiceKind.Default)] ILikeArticleCommentService service,
                         int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("LikeArticleComment_getLikedArticleComments")]
    public ListResponseBase<LikeArticleComment> GetItems(
                                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] ILikeArticleCommentService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }
}