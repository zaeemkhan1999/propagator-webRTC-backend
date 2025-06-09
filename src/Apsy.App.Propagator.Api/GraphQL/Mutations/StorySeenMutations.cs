namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class StorySeenMutations
{
    [GraphQLName("storySeen_createStorySeen")]
    public ResponseBase<StorySeen> CreateStorySeen(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStorySeenService service,
        StorySeenInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("storySeen_createStorySeens")]
    public ListResponseBase<StorySeen> CreateStorySeens(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStorySeenService service,
        List<StorySeenInput> input)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        foreach (var item in input)
        {
            item.UserId = currentUser.Id;
        }
        return service.AddSeens(input);
    }
}