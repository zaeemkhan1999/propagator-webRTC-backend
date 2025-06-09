namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class CommentInput : InputDef /*CommentInputDef*/
{

    public int PostId { get; set; }

    public int? ParentId { get; set; }
    public int? Id { get; set; }
    [GraphQLIgnore]
    public int? UserId { get; set; }
    public int? MentionId { get; set; }

    public string Text { get; set; }
    public CommentType CommentType { get; set; }
    public string ContentAddress { get; set; }
}