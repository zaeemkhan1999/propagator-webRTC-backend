using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IUsersSubscriptionReadService : IServiceBase<UsersSubscription, UsersSubscriptionInput>
    {
        Task<ResponseBase<UsersSubscriptionsFeaturesDto>> GetUsersSubscriptionsFeatures();
    }
}
