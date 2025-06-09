namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class SecurityAnswerMutations
{

    [GraphQLName("securityAnswer_createSecurityAnswer")]
    public async Task<ListResponseBase<SecurityAnswer>> CreateSecurityAnswer(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service] ISecurityAnswerService service,
         List<SecurityAnswerInput> input
        )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await service.AddSecurityAnswer(input,authentication.CurrentUser);
    }

    [GraphQLName("securityAnswer_updateSecurityAnswer")]
    public async Task<ResponseBase<SecurityAnswer>> UpdateSecurityAnswer(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service] ISecurityAnswerService userService,
          SecurityAnswerInput input
         )
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await userService.UpdateSecurityAnswer(input);
    }
}