using Aps.CommonBack.Base.Generics.Responses;
using Aps.CommonBack.Messaging.Generics.Responses;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SecretMessageInput : InputDef
{
    public int? SecretConversationId { get; set; }
    public virtual int? SenderId { get; set; }
    public virtual int? ReceiverId { get; set; }

    public string AesEncryptedMessageType { get; set; }
    public string AesEncryptedContentAddress { get; set; }
    public string AesEncryptedProfile { get; set; }
    public string AesEncryptedPost { get; set; }
    public string AesEncryptedArticle { get; set; }
    public string AesEncryptedStory { get; set; }
    public int? ParentMessageId { get; set; }

    public string PublicKey { get; set; }

    [GraphQLIgnore]
    public ResponseBase<SecretMessage> ValidateCreateMessage()
    {
        var senderId = SenderId;
        var receiverId = ReceiverId;

        if (senderId == receiverId)
        {
            return MessagingResponseStatus.SelfMessageNotAllowed;
        }
        if (senderId <= 0 || receiverId <= 0)
        {
            return ResponseStatus.NotEnoghData;
        }
        return ResponseStatus.Success;
    }

}
