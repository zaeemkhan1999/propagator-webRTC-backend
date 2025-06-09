using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Entities
{
    public class SecretConversation : EntityDef
    {
        public SecretConversation()
        {
            IsFirstUserDeleted = false;
            IsSecondUserDeleted = false;
        }


        public bool IsFirstUserDeleted { get; set; }
        public DateTime FirstUserDeletedDate { get; set; }

        public bool IsSecondUserDeleted { get; set; }
        public DateTime SecondUserDeletedDate { get; set; }

        public int LatestMessageUserId { get; set; }
        public DateTime? LatestMessageDate { get; set; }

        public bool IsBothUserJoinedToChat { get; set; }


        [Required]
        public int FirstUserId { get; set; }
        public User FirstUser { get; set; }
        public string FirstUserPublicKey { get; set; }
        public int SecondUnreadCount { get; set; }


        [Required]
        public int SecondUserId { get; set; }
        public User SecondUser { get; set; }
        public string SecondUserPublicKey { get; set; }
        public int FirstUnreadCount { get; set; }


        public bool IsSelfDestruct { get; set; }
        public DateTime? SelfDestructTime { get; set; }

        public ICollection<SecretMessage> SecretMessages { get; set; }
    }
}
