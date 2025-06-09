using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class LikeArticleCommentInput : BaseInputDef
{
    [GraphQLIgnore]
    public int? UserId { get; set; }

    //[Required(ErrorMessage = "{0} is required")]
    //public int? CommentId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int? ArticleCommentId { get; set; }
}