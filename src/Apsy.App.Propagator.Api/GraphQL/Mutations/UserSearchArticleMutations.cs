namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserSearchArticleMutations
{
    [GraphQLName("searchArticle_createSearchArticle")]
    public ResponseBase<UserSearchArticle> CreateSearchArticle(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] IUserSearchArticleService service,
            UserSearchArticleInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("searchArticle_removeSearchArticle")]
    public ResponseStatus RemoveSearchArticle(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] UserSearchArticleService service,
            int articleId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.DeleteSearchedArticle(currentUser.Id, articleId,authentication.CurrentUser);
    }
}