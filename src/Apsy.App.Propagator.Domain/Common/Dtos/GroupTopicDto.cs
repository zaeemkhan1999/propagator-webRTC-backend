namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class GroupTopicDto:DtoDef
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public string Title { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
