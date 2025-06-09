using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class UserSearchGroupDto : BaseDtoDef<int>
    {
        public Conversation Conversation { get; set; }
        public int ConversationId { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }
        public int MemberCount { get; set; }

    }
}
