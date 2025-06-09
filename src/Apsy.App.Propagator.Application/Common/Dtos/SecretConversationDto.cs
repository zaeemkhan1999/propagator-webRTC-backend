namespace Apsy.App.Propagator.Application.Common
{
    public class SecretConversationDto : DtoDef /*: ConversationDtoBase<User>*/
    {
        public SecretConversationDto() { }


        public int ConversationId { get; set; }

        public User User { get; set; }

        public int UnreadCount { get; set; }
        public DateTime? LatestMessageDate { get; set; }


        public SecretMessage LastMessage { get; set; }


        public bool IsFirstUserDeleted { get; set; }
        public DateTime FirstUserDeletedDate { get; set; }

        public bool IsSecondUserDeleted { get; set; }
        public DateTime SecondUserDeletedDate { get; set; }

        public int LatestMessageUserId { get; set; }



        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public DateTime? LastSeen { get; set; }
        public string ImageAddress { get; set; }
        public string Cover { get; set; }
        public UserTypes? UserTypes { get; set; }
    }
}
