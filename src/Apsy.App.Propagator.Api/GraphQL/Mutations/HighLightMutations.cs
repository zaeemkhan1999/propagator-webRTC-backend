namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class HighLightMutations
{
    [GraphQLName("highLight_createHighLight")]
    public ResponseBase<HighLight> CreateHighLight(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] IHighLightService service,
                    HighLightInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        input.UserId = currentUser.Id;
        return service.Add(input);
    }

    [GraphQLName("highLight_removeHighLight")]
    public ResponseStatus RemoveHighLight(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IHighLightService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.SoftDelete(entityId);
    }


    [GraphQLName("highLight_updateHighLight")]
    public ResponseBase<HighLight> Update(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service(ServiceKind.Default)] IHighLightService service,
        HighLightInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;

        input.UserId = currentUser.Id;
        return service.Update(input);
    }
}