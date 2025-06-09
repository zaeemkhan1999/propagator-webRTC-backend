namespace Apsy.App.Propagator.Application.Common.Inputs;

public class MessageInput : InputDef /*MessageInputDef*/
{

    public MessageType MessageType { get; set; }
    public string ContentAddress { get; set; }
    public int? ProfileId { get; set; }
    public int? PostId { get; set; }
    public int? ArticleId { get; set; }
    public int? StoryId { get; set; }
    public int? ParentMessageId { get; set; }

    public int? ConversationId { get; set; }

    public bool IsShare { get; set; }

    [GraphQLIgnore]
    public virtual int? SenderId { get; set; }
    public virtual int? ReceiverId { get; set; }
    public string Text { get; set; }

    public int? GroupTopicId { get; set; }
}