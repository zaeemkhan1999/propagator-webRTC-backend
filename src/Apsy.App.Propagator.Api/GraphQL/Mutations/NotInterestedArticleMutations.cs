namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class NotInterestedArticleMutations
{
    [GraphQLName("notInterestedArticle_addNotInterestedArticle")]
    public async Task<ResponseBase<NotInterestedArticle>> AddNotInterestedArticle(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] INotInterestedArticleService service,
                                NotInterestedArticleInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return await service.AddNotInterestedArticle(input);
    }

    [GraphQLName("notInterestedArticle_removeFromNotInterestedArticle")]
    public async Task<ResponseBase<NotInterestedArticle>> RemoveFromNotInterestedArticle(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] INotInterestedArticleService service,
                            NotInterestedArticleInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return await service.RemoveFromNotInterestedArticle(input);
    }
}
