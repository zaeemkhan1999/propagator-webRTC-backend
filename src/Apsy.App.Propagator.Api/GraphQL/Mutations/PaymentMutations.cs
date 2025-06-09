using Stripe;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class PaymentMutations
//: MutationBase<Payment, PaymentInput, IPaymentService, User>
{
    [GraphQLName("payment_isTransferEnabled")]
    public ResponseBase<bool> IsTransferEnabled(
                               [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                               [Service] IPaymentService _paymentService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        var res = _paymentService.IsTransferEnabled(currentUser.StripeAccountId);
        if (res == ResponseStatus.Success)
            return true;
        return false;
    }

    [GraphQLName("payment_onboardUserInStripeConnect")]
    public ResponseBase<string> OnboardUserInStripeConnect(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPaymentService _paymentService,
        [Service] IUserService userService,
        [Service] IConfiguration _configuration)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        string returnUrl = _configuration["BaseUrl"] + _configuration["stripeOnboardingReturnUrl"];
        string refreshUrl = _configuration["BaseUrl"] + _configuration["stripeOnboardingRefreshUrl"];

        var result = _paymentService.OnboardUserInStripeConnect(returnUrl, refreshUrl);

        if (result.Item1.Status != ResponseStatus.Success)
            return ResponseBase<string>.Failure(result.Item1.Status);

        currentUser.StripeAccountId = result.Item1.Result.Id;
        userService.Update(currentUser);

        return ResponseBase<string>.Success(result.Item2);
    }

    [GraphQLName("payment_createEphemeralKey")]
    public ResponseBase<EphemeralKeyDto> CreateEphemeralKey(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPaymentService _paymentService)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return _paymentService.CreateEphemeralKey();
    }

    [GraphQLName("payment_createStripeSubscription")]
    public async Task<ResponseBase<string>> CreateStripeSubscription(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPaymentService _paymentService,
        int subscriptionPlanId,
        EnvironmentType env)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return await _paymentService.CreateStripeSubscription(subscriptionPlanId, env);
    }

    [GraphQLName("payment_cancelSubscription")]
    public async Task<ResponseBase<bool>> CancelSubscription(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IPaymentService _paymentService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var result = await _paymentService.CancelSubscription();

        return result.Status == ResponseStatus.Success;
    }

}