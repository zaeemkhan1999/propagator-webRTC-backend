namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class ReportMutations
{
    [GraphQLName("report_report")]
    public ResponseBase<Report> Report(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                 ReportInput input,
                                 [Service] IReportService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.ReporterId = currentUser.Id;
        return service.Add(input);
    }
}