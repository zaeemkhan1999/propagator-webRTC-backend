using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class ReportInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public string Text { get; set; }
    public string Reason { get; set; }
    public string Email { get; set; }

    public ReportType ReportType { get; set; }

    [GraphQLIgnore]
    public int ReporterId { get; set; }

    public int? PostId { get; set; }
    public int? ArticleId { get; set; }
    public int? CommentId { get; set; }
    public int? ArticleCommentId { get; set; }
    public int? ReportedId { get; set; }
    public int? MessageId { get; set; }
}