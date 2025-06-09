using Aps.CommonBack.Base.Repositories;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class NotificationReadService : ServiceBase<Notification, NotificationInput>, INotificationReadService
    {
        private readonly INotificationReadRepository repository;
        private readonly FirebaseAppCreator _firebaseApp;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublisher _publisher;
        private readonly IUserReadRepository _userRepository;
        public NotificationReadService(
        INotificationReadRepository repository,
        FirebaseAppCreator firebaseApp,
        IPublisher publisher,
        IUserReadRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration) : base(repository)
        {
            this.repository = repository;
            _firebaseApp = firebaseApp;
            _publisher = publisher;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public ListResponseBase<NotificationDto> GetNotifs(User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var notifications = repository.GetNotifs(currentUser.Id);
            return new(notifications);
        }

        public ListResponseBase<Notification> GetNotifications(User currentUser)
        {
            var notifications = repository.GetNotifications(currentUser.Id);
            return ListResponseBase<Notification>.Success(notifications.AsQueryable());
        }
    }
}
