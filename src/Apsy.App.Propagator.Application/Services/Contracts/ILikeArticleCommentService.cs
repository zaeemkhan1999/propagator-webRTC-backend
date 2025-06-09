namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface ILikeArticleCommentService : IServiceBase<LikeArticleComment, LikeArticleCommentInput>
{
    ResponseBase<LikeArticleComment> UnLikeArticleComment(LikeArticleCommentInput input);
}