namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserMessageGroup : EntityDef
    {
        public bool IsAdmin { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public int UnreadCount { get; set; }
        //public DateTime MembershipExpirationTime { get; set; }

        /// <summary>
        /// Determines if a user added as a member to a group is allowed to publish any content
        /// </summary>
        public bool IsPublisher { get; set; }
    }
}