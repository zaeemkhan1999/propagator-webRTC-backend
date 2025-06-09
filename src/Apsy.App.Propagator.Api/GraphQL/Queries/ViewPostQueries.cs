namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class ViewPostQueries
{
    [GraphQLName("viewPost_getViewPost")]
    public virtual SingleResponseBase<UserViewPost> Get(
                          [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                          [Service(ServiceKind.Default)] IViewPostService service,
                          int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.Get(entityId);
    }

    [GraphQLName("viewPost_getViewPosts")]
    public ListResponseBase<UserViewPost> GetItems(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] IViewPostService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.Get();
    }
}