using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface INotificationReadService : IServiceBase<Notification, NotificationInput>
    {
        ListResponseBase<NotificationDto> GetNotifs(User currentUser);
        ListResponseBase<Notification> GetNotifications(User currentUser);
    }
}
