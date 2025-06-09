using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SecurityAnswerQueries
{
    [GraphQLName("securityAnswer_getSecurityAnswers")]
    public async Task<ListResponseBase<SecurityAnswer>> GetSecurityAnswers(
                            // [Authentication] RequestInterception.Authentication authentication,
                            string username, string password,
                            [Service(ServiceKind.Default)] ISecurityAnswerReadService service)
    {

        //if (authentication.Status != ResponseStatus.Success)
        //{
        //    return authentication.Status;
        //}
        return await service.GetSecurityAnswerCurrentUser(username, password);
    }

    [GraphQLName("securityAnswer_getSecurityAnswersByToken")]
    public async Task<ListResponseBase<SecurityAnswer>> GetSecurityAnswersByToken(
                             [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                            [Service(ServiceKind.Default)] ISecurityAnswerReadService service)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.GetSecurityAnswerCurrentUser("", "", authentication.CurrentUser.Id);
    }
}
