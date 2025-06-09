namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class NotificationEventDto
    {
        public Notification Notification { get; set; }
        public User Sender { get; set; }
        public User Reciever { get; set; }
    }
}
