namespace Apsy.App.Propagator.Application.Common
{
    public class ConversationDto : DtoDef /*ConversationDtoBase<User>*/
    {
        public ConversationDto() { }


        public int ConversationId { get; set; }

        public User User { get; set; }

        public int UnreadCount { get; set; }
        public DateTime? LatestMessageDate { get; set; }





        public bool IsGroup { get; set; }

        public User Admin { get; set; }
        public int? AdminId { get; set; }


        public string GroupDescription { get; set; }
        public string GroupLink { get; set; }
        public bool IsPrivate { get; set; }

        public string GroupName { get; set; }

        public string GroupImgageUrl { get; set; }

        public Message LastMessage { get; set; }


        public bool IsFirstUserDeleted { get; set; }
        public DateTime FirstUserDeletedDate { get; set; }

        public bool IsSecondUserDeleted { get; set; }
        public DateTime SecondUserDeletedDate { get; set; }

        public int LatestMessageUserId { get; set; }




        public int? UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public DateTime? LastSeen { get; set; }
        public string ImageAddress { get; set; }
        public string Cover { get; set; }
        public UserTypes? UserTypes { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Bio { get; set; }

        public int GroupMemberCount { get; set; }
        public bool IsMemberOfGroup { get; set; }
    }
}