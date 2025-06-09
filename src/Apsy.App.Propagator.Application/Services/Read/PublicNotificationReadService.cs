using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class PublicNotificationReadService: ServiceBase<PublicNotification, PublicNotificationInput>, IPublicNotificationReadService
    {
        private readonly IPublicNotificationRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublisher _publisher;
        private readonly IUserReadRepository userRepository;
        public PublicNotificationReadService(
        IPublicNotificationRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IUserReadRepository userRepository) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _publisher = publisher;
            this.userRepository = userRepository;
        }
        public SingleResponseBase<PublicNotification> GetPublicNotification(int id)
        {
            //var query = repository.Where(d => d.Id == id);
            var query = repository.GetPublicNotification(id);

            return SingleResponseBase.Success(query);
        }

        public ListResponseBase<PublicNotification> GetPublicNotifications()
        {
            //var query = repository.GetDbSet();
            var query = repository.GetPublicNotifications();

            return ListResponseBase.Success(query);
        }
    }
}
