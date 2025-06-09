using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class ArticleCommentInput : BaseInputDef
{
    [GraphQLIgnore]
    public int? UserId { get; set; }
    public int? MentionId { get; set; }
    public string Text { get; set; }
    public CommentType CommentType { get; set; }
    public string ContentAddress { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int? ArticleId { get; set; }

    public int? ParentId { get; set; }
}
