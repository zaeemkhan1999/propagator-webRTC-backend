using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IPublicNotificationReadService: IServiceBase<PublicNotification, PublicNotificationInput>
    {
        SingleResponseBase<PublicNotification> GetPublicNotification(int id);
        ListResponseBase<PublicNotification> GetPublicNotifications();
    }
}
