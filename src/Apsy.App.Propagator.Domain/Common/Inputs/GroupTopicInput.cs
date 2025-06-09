namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

    public class GroupTopicInput: InputDef
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public string Title { get; set; }

        public ICollection<Message> Messages { get; set; }
    }

