using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PaymentQueries
{

    [GraphQLName("payment_getAllPayments")]
    public ListResponseBase<Payment> GetAllPayments(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service] IPaymentReadService _paymentService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ListResponseBase<Payment>.Failure(ResponseStatus.AuthenticationFailed);
        return _paymentService.Get();
    }

    [GraphQLName("payment_getPublishableKey")]
    public ResponseBase<string> GetPublishableKey(
                                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                    [Service] IPaymentReadService _paymentService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return _paymentService.GetPublishableKey();
    }

    [GraphQLName("payment_hasStripeAccount")]
    public ResponseBase<bool> HasStripeAccount(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPaymentReadService _paymentService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return _paymentService.HasStripeAccount(currentUser.StripeAccountId);
    }

    [GraphQLName("payment_getStripeAccountRequirements")]
    public ListResponseBase<StripeAccountRequirementsError> GetStripeErrors(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPaymentReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return service.GetAccountRequirements(currentUser);
    }
    [GraphQLName("payment_getStripeSession")]
    public async Task<ResponseBase<string>> GetStripeSession(
   [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
   [Service] IPaymentReadService service, string priceid, string baseurl, string userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        int subscriptionPlanId = service.GetSubscriptionPlanIdByPriceId(priceid);
        return await service.CreateCheckoutSessionAsync(priceid, baseurl, userId, subscriptionPlanId);
    }


}