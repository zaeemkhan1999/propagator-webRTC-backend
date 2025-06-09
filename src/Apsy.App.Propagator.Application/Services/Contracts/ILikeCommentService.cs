namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface ILikeCommentService
 : IServiceBase<LikeComment, LikeCommentInput>
{

    #region functions
    #endregion
    ResponseBase<LikeComment> UnlikeComment(LikeCommentInput input);
}
