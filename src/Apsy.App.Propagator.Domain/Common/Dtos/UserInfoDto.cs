using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class UserInfoDto : DtoDef
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string ImageAddress { get; set; }
        public string Cover { get; set; }

        public Notification Notification { get; set; }

        public GraphqlSubscriptionType GraphqlSubscriptionType { get; set; }
        public Message Message { get; set; }
        public UserInfoDto(Message message)
        {
            GraphqlSubscriptionType = GraphqlSubscriptionType.Message;
            Message = message;
        }

        public UserInfoDto(Notification notification)
        {
            GraphqlSubscriptionType = GraphqlSubscriptionType.Notification;
            Notification = notification;
        }
    }
}