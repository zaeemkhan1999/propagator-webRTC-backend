namespace Apsy.App.Propagator.Application.Common
{
    public class MessageDto : BaseDtoDef<int>
    {
        public DateTime CreatedAt { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public string Text { get; set; }



        public bool IsSenderDeleted { get; set; }
        public bool IsReceiverDeleted { get; set; }

        public MessageType MessageType { get; set; }
        public string ContentAddress { get; set; }
        public int? GroupId { get; set; }
        public bool IsEdited { get; set; } = false;
        public bool IsSeen { get; set; }
        public DateTime? SeenDate { get; set; }

        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }


        public int? ProfileId { get; set; }
        public User Profile { get; set; }

        public Post Post { get; set; }
        public int? PostId { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }

        public Story Story { get; set; }
        public int? StoryId { get; set; }

        public Message ParentMessage { get; set; }
        public int? ParentMessageId { get; set; }
        public bool IsDelivered { get; set; }

    }
}