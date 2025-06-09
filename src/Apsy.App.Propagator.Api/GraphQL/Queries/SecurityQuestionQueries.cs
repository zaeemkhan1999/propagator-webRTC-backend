namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class SecurityQuestionQueries
{
    [GraphQLName("securityQuestion_getSecurityQuestions")]
    public ListResponseBase<SecurityQuestion> Get(
                                        [Service(ServiceKind.Default)] ISecurityAnswerService service)
    {

        return service.Get<SecurityQuestion>();
    }


}