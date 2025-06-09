//using Aps.CommonBack.Payments.Services.Contracts;


using Propagator.Common.Services.Contracts;
using Stripe;

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IPaymentService : IStripeSerivceBase<Payment, StripPaymentInput, User, CustomerInput, StripFullPaymentInput>
{
    public ResponseBase<Account> CreateConnectAccount(User currentUser);
    ResponseBase<bool> Withdrawal(double tokenCount);
    ResponseBase<BuyTokenDto> BuyToken(double tokenCount);
    ValueTask UpdatePaymentConfirmationStatus(int paymentId, string paymentIntentId, PaymentConfirmationStatus paymentConfirmationStatus);
    (ResponseBase<Account>, string) OnboardUserInStripeConnect(string returnUrl, string refreshUrl);
    Task ConfirmPostPayment(int postId, int adsId, string paymentIntentId, int paymentId, PaymentConfirmationStatus paymentConfirmationStatus, PaymentStatus paymentStatus);
    Task FailPostPayment(int postId, int adsId, string paymentIntentId, int paymentId, PaymentConfirmationStatus paymentConfirmationStatus, PaymentStatus paymentStatus);

    Task ConfirmArticlePayment(int articleId, int adsId, string paymentIntentId, int paymentId, PaymentConfirmationStatus paymentConfirmationStatus, PaymentStatus paymentStatus);
    Task FailArticlePayment(int articleId, int adsId, string paymentIntentId, int paymentId, PaymentConfirmationStatus paymentConfirmationStatus, PaymentStatus paymentStatus);

    ResponseBase<EphemeralKeyDto> CreateEphemeralKey();
    ResponseBase<PaymentIntent> CreatePaymentIntent(PaymentIntentInput input);
    Task<ResponseBase<string>> CreateStripeSubscription(int subscriptionPlanId, EnvironmentType env);
    Task<ResponseBase<string>> CheckoutSessionAsync(string priceId, string baseUrl, string userId, string customerId = null);
    Task<ResponseBase<Stripe.Subscription>> CancelSubscription();
}