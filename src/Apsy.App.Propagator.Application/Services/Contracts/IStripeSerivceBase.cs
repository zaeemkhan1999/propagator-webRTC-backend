using Stripe;

namespace Propagator.Common.Services.Contracts
{
    [NonInjectabel]
    public interface IStripeSerivceBase<TPayment, TPaymentInput, TUser, TCustomerInput, TStripeFullPayModel> : IServiceBase<TPayment, TPaymentInput> where TPayment : EntityDef where TPaymentInput : InputDef where TUser : UserDef where TCustomerInput : InputDef where TStripeFullPayModel : InputDef
    {
        Task<ResponseBase<ChargeDto>> CreateCharge(TPaymentInput paymentInput, string currency = "usd");

        ResponseStatus CreateChargeByFullData(TStripeFullPayModel paymentInput);

        ResponseStatus SubscribeToPlan(string planId, string customerId);

        Task<ResponseBase<string>> CreateCheckoutSessionAsync(string priceId, string baseUrl, string userId, int subscriptionPlanId, string customerId = null);

        ResponseBase<CustomerDto> CreateCustomer(TCustomerInput customerInput);

        ResponseBase<TransferDto> TransferMoneyToAccount(string userStripeAccountId, double cost, Dictionary<string, string> metadata, string currency = "usd", string transferGroup = null);

        ResponseBase<ChargeDto> GetMoneyFromConnectAccount(string userStripeAccountId, double cost, Dictionary<string, string> metadata, string currency = "usd");

        ResponseStatus IsTransferEnabled(string userStripeAccountId);

        ResponseBase<bool> HasStripeAccount(string userStripeAccountId);

        ResponseBase<long> GetPlatformBlance();

        ResponseBase<bool> HasEnoughBalanceForConnectUser(double amount, string userStripeAccountId);

        ResponseBase<bool> HasEnoughBalanceForPlatform(double amount);

        ResponseBase<long> GetConnectUserBlance(string userStripeAccountId);

        ListResponseBase<Charge> GeCharges();

        ListResponseBase<ChargeDto> GeChargesData();

        ResponseBase<Balance> GeBalance(string stripeConnectedUserId = null);

        ResponseBase<BalanceDto> GeBalanceData(string stripeConnectedUserId = null);

        ListResponseBase<Customer> GeCustomers();

        ListResponseBase<CustomerDto> GeCustomersData();

        ListResponseBase<BalanceTransaction> GeTransactions();

        ListResponseBase<BalanceTransactionDto> GeTransactionsData();

        ListResponseBase<Dispute> GeDisputes();

        ListResponseBase<DisputeDto> GeDisputesData();

        ListResponseBase<Refund> GeRefunds();

        ListResponseBase<RefundDto> GeRefundsData();

        ListResponseBase<Plan> GePlans();

        ListResponseBase<PlanDto> GePlansData();

        ResponseBase<Account> GetConnectedUser(string stripeConnectedUserId);

        ResponseBase<AccountDto> GetConnectedUserData(string stripeConnectedUserId);

    }
}
