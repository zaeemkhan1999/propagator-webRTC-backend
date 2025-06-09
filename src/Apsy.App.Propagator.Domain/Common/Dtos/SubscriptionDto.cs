namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class SubscriptionDto : DtoDef
    {
        public SubscriptionDto()
        {
            
        }

        public SubscriptionDto(Message message)
        {
            GraphqlSubscriptionType = GraphqlSubscriptionType.Message;
            Message = message;
        }

        public SubscriptionDto(Notification notification)
        {
            GraphqlSubscriptionType = GraphqlSubscriptionType.Notification;
            Notification = notification;
        }

        public SubscriptionDto(User user)
        {
            GraphqlSubscriptionType = GraphqlSubscriptionType.User;
            User = user;
        }

        public GraphqlSubscriptionType GraphqlSubscriptionType { get; set; }
        public Message Message { get; set; }
        public Notification Notification { get; set; }

        public User User {get; set; }
    }
}