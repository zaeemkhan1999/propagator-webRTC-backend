namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class SecretMessageDto : BaseDtoDef<int>
    {

        public SecretConversation SecretConversation { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public string AesEncryptedText { get; set; }

        public string AesKeyEncryptedByRsaUsingSenderPublicKey { get; set; }
        public string AesIvEncryptedByRsaUsingSenderPublicKey { get; set; }

        public string AesKeyEncryptedByRsaUsingReceiverPublicKey { get; set; }
        public string AesIvEncryptedByRsaUsingReceiverPublicKey { get; set; }


        public bool IsSenderDeleted { get; set; }
        public bool IsReceiverDeleted { get; set; }

        public string AesEncryptedMessageType { get; set; }
        public string AesEncryptedContentAddress { get; set; }
        public bool IsSeen { get; set; }

        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }


        public string AesEncryptedProfile { get; set; }
        public string AesEncryptedPost { get; set; }
        public string AesEncryptedArticle { get; set; }
        public string AesEncryptedStory { get; set; }

        public SecretMessage ParentSecretMessage { get; set; }
        public int? ParentMessageId { get; set; }

    }
}
