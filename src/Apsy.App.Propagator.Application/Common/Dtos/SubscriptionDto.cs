namespace Apsy.App.Propagator.Application.Common
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

        public GraphqlSubscriptionType GraphqlSubscriptionType { get; set; }
        public Message Message { get; set; }
        public Notification Notification { get; set; }
    }
}