using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface ISecretMessageReadService : IServiceBase<SecretMessage, SecretMessageInput>
    {
        ListResponseBase<SecretMessageDto> GetDirectMessages(User currentUser);
        SingleResponseBase<SecretConversation> GetConversation(int conversationId, int userId);
        ListResponseBase<SecretConversationDto> GetUserMessages(int userId, string publicKey, User currentUser);
        SingleResponseBase<SecretConversation> GetConversationWithOtherUser(int otherUserId, User currentUser);
    }
}
