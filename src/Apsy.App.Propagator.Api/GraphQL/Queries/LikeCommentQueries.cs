namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class LikeCommentQueries
{

    [GraphQLName("likeComment_getLikedComment")]
    public SingleResponseBase<LikeComment> Get(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                         [Service(ServiceKind.Default)] ILikeCommentService service,
                         int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("likeComment_getLikedComments")]
    public ListResponseBase<LikeComment> GetItems(
                                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] ILikeCommentService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.Get();
    }
}