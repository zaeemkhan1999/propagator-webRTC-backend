namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class NotInterestedPostMutations
{
    [GraphQLName("notInterestedPost_addNotInterestedPost")]
    public async Task<ResponseBase<NotInterestedPost>> AddNotInterestedPost(
                               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service(ServiceKind.Default)] INotInterestedPostService service,
                                NotInterestedPostInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return await service.AddNotInterested(input);
    }

    [GraphQLName("notInterestedPost_removeFromNotInterestedPost")]
    public async Task<ResponseBase<NotInterestedPost>> RemoveFromNotInterestedPost(
                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] INotInterestedPostService service,
                            NotInterestedPostInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return await service.RemoveFromNotInterestedPost(input);
    }
}
