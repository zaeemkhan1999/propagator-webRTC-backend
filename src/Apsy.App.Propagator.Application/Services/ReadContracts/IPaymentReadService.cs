using Propagator.Common.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IPaymentReadService : IStripeSerivceBase<Payment, StripPaymentInput, User, CustomerInput, StripFullPaymentInput>
    {
        ResponseBase<string> GetPublishableKey();
        ResponseBase<ViewPriceDto> GetViewPrice();
        ListResponseBase<StripeAccountRequirementsError> GetAccountRequirements(User currentUser);
        int GetSubscriptionPlanIdByPriceId(string priceId);
    }
}
