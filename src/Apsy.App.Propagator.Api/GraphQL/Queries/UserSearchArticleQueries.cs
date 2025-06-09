namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserSearchArticleQueries
{
    [GraphQLName("userSearchArticle_getUserSearchArticle")]
    public SingleResponseBase<UserSearchArticle> Get(
                                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                    [Service(ServiceKind.Default)] IUserSearchArticleService service,
                                    int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }
    [GraphQLName("userSearchArticle_getUserSearchArticles")]
    public ListResponseBase<UserSearchArticle> GetItems(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchArticleService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get();
    }
}