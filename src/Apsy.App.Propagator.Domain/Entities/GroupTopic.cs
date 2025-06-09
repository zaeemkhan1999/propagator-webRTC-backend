namespace Apsy.App.Propagator.Domain.Entities;

public class GroupTopic : EntityDef
{
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }

    public string Title { get; set; }

    public ICollection<Message> Messages { get; set; }
}
