namespace Apsy.App.Propagator.Domain.Entities;

public class UserSearchGroup : UserKindDef<User>
{
    public Conversation Conversation { get; set; }
    public int ConversationId { get; set; }
}