namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class VerificationRequestMutations
{
    [GraphQLName("verificationRequest_CreateVerificationRequest")]
    public ResponseBase<VerificationRequest> Create(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service(ServiceKind.Default)] IVerificationRequestService service,
                VerificationRequestInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;

        input.VerificationRequestAcceptStatus = VerificationRequestAcceptStatus.Pending;
        return service.Add(input);
    }


    [GraphQLName("verificationRequest_acceptVerificationRequest")]
    public async Task<ResponseBase<VerificationRequest>> ConfirmationVerificationRequest(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IVerificationRequestService service,
                        int requestId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;


        return await service.AcceptedVerificationRequest(requestId);
    }

    [GraphQLName("verificationRequest_rejectVerificationRequest")]
    public async Task<ResponseBase<VerificationRequest>> RejectVerificationRequest(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IVerificationRequestService service,
                        int requestId, string reason)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;


        return await service.RejectVerificationRequest(requestId, reason);
    }

    [GraphQLName("verificationRequest_removeVerificationRequest")]
    public async Task<ResponseBase> RemoveVerificationRequest(
                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                [Service(ServiceKind.Default)] IVerificationRequestService service,
                int requestId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;


        return await service.RemoveVerificationRequest(requestId);
    }
}