namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserSearchPlaceMutations
{
    [GraphQLName("userSearchPlace_createUserSearchPlace")]
    public ResponseBase<UserSearchPlace> Create(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserSearchPlaceService service,
        UserSearchPlaceInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("userSearchPlace_removeUserSearchPlace")]
    public ResponseStatus Remove(
            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
            [Service(ServiceKind.Default)] UserSearchPlaceService service,
            string place)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return service.DeleteSearchedPlace(currentUser.Id, place);
    }
}
